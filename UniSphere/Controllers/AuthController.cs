using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.Entities;
using UniSphere.Api.Services;

namespace UniSphere.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public sealed class AuthController(
ApplicationDbContext applicationDbContext,
UserManager<ApplicationUser> userManager,
ApplicationIdentityDbContext identityDbContext,
TokenProvider tokenProvider
) : ControllerBase
{

    [HttpGet("CheckOneTimeCode")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]

    public async Task<ActionResult<SimpleStudentDto>> CheckOneTimeCode(string studentNumber, int code, Guid majorId)
    {
        StudentCredential studentCredential = await applicationDbContext.StudentCredentials
        .Include(sc => sc.Major)
        .FirstOrDefaultAsync(sc => sc.StudentNumber == studentNumber && sc.MajorId == majorId);
        if (studentCredential is null)
        {
            return NotFound();
        }
        if (!IsCodeValid(studentCredential, code))
        {
            return NotFound();
        }
        return Ok(studentCredential.ToSimpleStudentDto());
    }
    [HttpPost("Register/Student")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FullInfoStudentDto>> Register(RegisterStudentDto registerStudentDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        StudentCredential studentCredential = await applicationDbContext.StudentCredentials.Include(sc => sc.EnrollmentStatus)
        .Include(sc => sc.Major)
        .FirstOrDefaultAsync(sc => sc.Id == registerStudentDto.StudentId);
        if (studentCredential is null)
        {
            return NotFound();
        }
        var applicationUser = new ApplicationUser
        {
            UserName = registerStudentDto.StudentId.ToString(),
            StudentId = registerStudentDto.StudentId,
        };
        if (registerStudentDto.Password != registerStudentDto.ConfirmPassword)
        {
            return BadRequest("Password and ConfirmPassword must be the same");
        }
        IdentityResult identityResult = await userManager.CreateAsync(applicationUser, registerStudentDto.Password);
        if (!identityResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", identityResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };
            1
        }

        
        studentCredential.IdentityId = applicationUser.Id;
        await applicationDbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(registerStudentDto.StudentId)
            );

        return Ok(studentCredential.ToFullInfoStudentDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }
    [HttpPost("Login/Student")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    
    public async Task<ActionResult<FullInfoStudentDto>> Login(LoginStudentDto loginStudentDto)
    {
        StudentCredential studentCredential = await applicationDbContext.StudentCredentials.Include(sc => sc.EnrollmentStatus)
            .Include(sc => sc.Major)
            .FirstOrDefaultAsync(sc => sc.StudentNumber == loginStudentDto.StudentNumber  && sc.MajorId == loginStudentDto.MajorId);
        if (studentCredential is null)
        {
            return Problem(
                    detail: "Student not found",
                    title: "Student not found",
                    statusCode: StatusCodes.Status404NotFound
                );
        }

      
        ApplicationUser? applicationUser = await userManager.Users.FirstOrDefaultAsync(u => u.StudentId == studentCredential.Id);
        if (applicationUser is null || !await userManager.CheckPasswordAsync(applicationUser, loginStudentDto.Password))
        {
            return Problem(
                detail : "Student not Registered",
                title : "Student not Registered",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(studentCredential.Id)
        );

        return Ok(studentCredential.ToFullInfoStudentDto(accessTokens.AccessToken, accessTokens.RefreshToken));

    }

    private static bool IsCodeValid(StudentCredential studentCredential, int code)
    {
        if (studentCredential.OneTimeCodeCreatedDate is null || studentCredential.OneTimeCodeExpirationInMinutes is null || studentCredential.OneTimeCode is null)
        {
            return false;
        }
        if (studentCredential.OneTimeCode != code)
        {
            return false;
        }

        if (studentCredential.OneTimeCodeCreatedDate!.Value.AddMinutes(studentCredential.OneTimeCodeExpirationInMinutes!.Value) < DateTime.UtcNow)
        {
            return false;
        }
        return true;
    }
}
