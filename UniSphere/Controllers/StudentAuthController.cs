using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.Entities;
using UniSphere.Api.Services;
using UniSphere.Api.Helpers;

namespace UniSphere.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public sealed class StudentAuthController(
    ApplicationDbContext applicationDbContext,
    UserManager<ApplicationUser> userManager,
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider,
    IAuthService authService
) : BaseController
{
    [HttpPost("CheckOneTimeCode")]
    public async Task<ActionResult<SimpleStudentDto>> CheckOneTimeCode(StudentCheckOneTimeCodeDto checkOneTimeCodeDto)
    {
        var studentCredential = await applicationDbContext.StudentCredentials
            .Include(sc => sc.Major)
            .FirstOrDefaultAsync(sc => sc.StudentNumber == checkOneTimeCodeDto.StudentNumber && sc.MajorId == checkOneTimeCodeDto.MajorId);
        
        if (studentCredential is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetStudentNotFoundMessage(Lang) });
        }

        if (!await authService.ValidateOneTimeCodeAsync(studentCredential, checkOneTimeCodeDto.Code))
        {
            return BadRequest(new { message = BilingualErrorMessages.GetInvalidCodeMessage(Lang) });
        }

        return Ok(studentCredential.ToSimpleStudentDto());
    }

    [HttpPost("Register")]
    public async Task<ActionResult<FullInfoStudentDto>> Register(RegisterStudentDto registerStudentDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        StudentCredential? studentCredential = await applicationDbContext.StudentCredentials
            .Include(sc => sc.EnrollmentStatus)
            .Include(sc => sc.Major)
            .FirstOrDefaultAsync(sc => sc.Id == registerStudentDto.StudentId);
        
        if (studentCredential is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetStudentNotFoundMessage(Lang) });
        }

        var applicationUser = new ApplicationUser
        {
            UserName = registerStudentDto.StudentId.ToString(),
            StudentId = registerStudentDto.StudentId,
        };
        
        if (registerStudentDto.Password != registerStudentDto.ConfirmPassword)
        {
            return BadRequest(new { message = BilingualErrorMessages.GetPasswordMismatchMessage(Lang) });
        }

        IdentityResult createStudentResult = await userManager.CreateAsync(applicationUser, registerStudentDto.Password);
        if (!createStudentResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createStudentResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: BilingualErrorMessages.GetErrorCreatingStudentMessage(Lang),
                title: BilingualErrorMessages.GetErrorCreatingStudentMessage(Lang),
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(applicationUser, Roles.Student);
        if (!addRoleResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createStudentResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: BilingualErrorMessages.GetErrorCreatingStudentMessage(Lang),
                title: BilingualErrorMessages.GetErrorCreatingStudentMessage(Lang),
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        
        studentCredential.IdentityId = applicationUser.Id;
        
        await applicationDbContext.SaveChangesAsync();
        
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest([Roles.Student], registerStudentDto.StudentId)
        );
        await authService.CreateOrUpdateRefreshTokenAsync(applicationUser, accessTokens.RefreshToken);

        await identityDbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        
        return Ok(studentCredential.ToFullInfoStudentDto(accessTokens.AccessToken, accessTokens.RefreshToken, Roles.Student));
    }

    [HttpPost("Login")]
    public async Task<ActionResult<FullInfoStudentDto>> Login(LoginStudentDto loginStudentDto)
    {
        StudentCredential? studentCredential = await applicationDbContext.StudentCredentials
            .Include(sc => sc.EnrollmentStatus)
            .Include(sc => sc.Major)
            .FirstOrDefaultAsync(sc =>
                sc.StudentNumber == loginStudentDto.StudentNumber && sc.MajorId == loginStudentDto.MajorId);
        
        if (studentCredential is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetStudentNotFoundMessage(Lang) });
        }

        ApplicationUser? applicationUser =
            await userManager.Users.FirstOrDefaultAsync(u => u.StudentId == studentCredential.Id);
        if (applicationUser is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetStudentNotRegisteredMessage(Lang) });
        }

        if (!await userManager.CheckPasswordAsync(applicationUser, loginStudentDto.Password))
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetWrongPasswordMessage(Lang) });
        }

        IList<string> roles = await userManager.GetRolesAsync(applicationUser);
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(roles, studentCredential.Id)
        );
        
        await authService.CreateOrUpdateRefreshTokenAsync(applicationUser, accessTokens.RefreshToken);
        
        return Ok(studentCredential.ToFullInfoStudentDto(accessTokens.AccessToken, accessTokens.RefreshToken, Roles.Student));
    }
} 
