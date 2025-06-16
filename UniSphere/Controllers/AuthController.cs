using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.Entities;
using UniSphere.Api.Services;
using UniSphere.Api.Settings;

namespace UniSphere.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]

[Produces("application/json")]
public sealed class AuthController(
    ApplicationDbContext applicationDbContext,
    UserManager<ApplicationUser> userManager,
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider,
    IOptions<JwtAuthOptions> options
) : ControllerBase
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    [HttpGet("Student/CheckOneTimeCode")]

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

    [HttpPost("Student/Register")]
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

        IdentityResult createStudentResult = await userManager.CreateAsync(applicationUser, registerStudentDto.Password);
        if (!createStudentResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createStudentResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating Student",
                title: "Error creating Student",
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
                detail: "Error creating Student",
                title: "Error creating Student",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        studentCredential.IdentityId = applicationUser.Id;
        await applicationDbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(registerStudentDto.StudentId, [Roles.Student]
        ));
        await identityDbContext.SaveChangesAsync();
        return Ok(studentCredential.ToFullInfoStudentDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }

    [HttpPost("Student/Login")]
    public async Task<ActionResult<FullInfoStudentDto>> Login(LoginStudentDto loginStudentDto)
    {
        StudentCredential? studentCredential = await applicationDbContext.StudentCredentials
            .Include(sc => sc.EnrollmentStatus)
            .Include(sc => sc.Major)
            .FirstOrDefaultAsync(sc =>
                sc.StudentNumber == loginStudentDto.StudentNumber && sc.MajorId == loginStudentDto.MajorId);
        if (studentCredential is null)
        {
            return Problem(
                detail: "Student not found",
                title: "Student not found",
                statusCode: StatusCodes.Status404NotFound
            );
        }


        ApplicationUser? applicationUser =
            await userManager.Users.FirstOrDefaultAsync(u => u.StudentId == studentCredential.Id);
        if (applicationUser is null )
        {
            return Problem(
                detail: "Student not Registered",
                title: "Student not Registered",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        if (!await userManager.CheckPasswordAsync(applicationUser, loginStudentDto.Password))
        {
            return Problem(
                detail: "Wrong password",
                title: "Wrong password",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        IList<string> roles = await userManager.GetRolesAsync(applicationUser);
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(studentCredential.Id,roles)
        );
        RefreshToken? refreshToken = await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.UserId == studentCredential.IdentityId);
        if (refreshToken is null)
        {
            var newRefreshToken = new RefreshToken
            {
                Id = Guid.CreateVersion7(),
                UserId = applicationUser.Id,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays),
                Token = accessTokens.RefreshToken
            };
            await identityDbContext.RefreshTokens.AddAsync(newRefreshToken);
        }
        else
        {
            refreshToken.Token = accessTokens.RefreshToken;
            refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays);
        }
        await identityDbContext.SaveChangesAsync();
        return Ok(studentCredential.ToFullInfoStudentDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }
        [HttpPost("RefreshToken")]
        public async Task<ActionResult<AccessTokensDto>> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            RefreshToken? refreshToken = await identityDbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken);
            if (refreshToken is null)
            {
                return Unauthorized();
            }
            if (refreshToken.ExpiresAtUtc < DateTime.UtcNow)
            {
                return Unauthorized();
            }
            IList<string> roles = await userManager.GetRolesAsync(refreshToken.User);

            AccessTokensDto accessTokens = tokenProvider.Create(
                new TokenRequest(refreshToken.User.StudentId, roles)
            );
            refreshToken.Token = accessTokens.RefreshToken;
            refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays);
            await identityDbContext.SaveChangesAsync();
          
            return Ok(accessTokens);
        }

       

        private static bool IsCodeValid(StudentCredential studentCredential, int code)
    {
        if (studentCredential.OneTimeCodeCreatedDate is null ||
            studentCredential.OneTimeCodeExpirationInMinutes is null || studentCredential.OneTimeCode is null)
        {
            return false;
        }

        if (studentCredential.OneTimeCode != code)
        {
            return false;
        }

        if (studentCredential.OneTimeCodeCreatedDate!.Value.AddMinutes(studentCredential.OneTimeCodeExpirationInMinutes!
                .Value) < DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }
}
