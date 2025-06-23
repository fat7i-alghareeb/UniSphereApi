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
public sealed class AuthController(
    ApplicationDbContext applicationDbContext,
    UserManager<ApplicationUser> userManager,
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider,
    IOptions<JwtAuthOptions> options
) : BaseController
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    [HttpPost("Student/CheckOneTimeCode")]

    public async Task<ActionResult<SimpleStudentDto>> CheckOneTimeCode(StudentCheckOneTimeCodeDto checkOneTimeCodeDto)
    {
      
        
        var studentCredential = await applicationDbContext.StudentCredentials
            .Include(sc => sc.Major)
            .FirstOrDefaultAsync(sc => sc.StudentNumber == checkOneTimeCodeDto.StudentNumber && sc.MajorId == checkOneTimeCodeDto.MajorId);
        if (studentCredential is null )
        {
            return NotFound();
        }

        if (!IsCodeValid(studentCredential, checkOneTimeCodeDto.Code))
        {
            return Problem(
                detail: "Code is not valid",
                title: "Code is not valid",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        return Ok(studentCredential.ToSimpleStudentDto(Lang));
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
            new TokenRequest([Roles.Student], registerStudentDto.StudentId)
        );
        await identityDbContext.SaveChangesAsync();
        return Ok(studentCredential.ToFullInfoStudentDto(accessTokens.AccessToken, accessTokens.RefreshToken,Lang));
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
            new TokenRequest(roles, studentCredential.Id)
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
        return Ok(studentCredential.ToFullInfoStudentDto(accessTokens.AccessToken, accessTokens.RefreshToken,Lang));
    }

    // Admin Authentication Endpoints
    [HttpPost("Admin/CheckOneTimeCode")]
    public async Task<ActionResult<SimpleAdminDto>> CheckAdminOneTimeCode(AdminCheckOneTimeCodeDto checkOneTimeCodeDto)
    {
        var admin = await applicationDbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Gmail == checkOneTimeCodeDto.Gmail && a.MajorId == checkOneTimeCodeDto.MajorId);
        if (admin is null)
        {
            return NotFound();
        }

        if (!IsAdminCodeValid(admin, checkOneTimeCodeDto.Code))
        {
            return Problem(
                detail: "Code is not valid",
                title: "Code is not valid",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        return Ok(admin.ToSimpleAdminDto(Lang));
    }

    [HttpPost("Admin/Register")]
    public async Task<ActionResult<FullInfoAdminDto>> RegisterAdmin(RegisterAdminDto registerAdminDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        Admin? admin = await applicationDbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Id == registerAdminDto.AdminId);
        if (admin is null)
        {
            return NotFound();
        }

        var applicationUser = new ApplicationUser
        {
            UserName = registerAdminDto.AdminId.ToString(),
            AdminId = registerAdminDto.AdminId,
        };
        if (registerAdminDto.Password != registerAdminDto.ConfirmPassword)
        {
            return BadRequest("Password and ConfirmPassword must be the same");
        }

        IdentityResult createAdminResult = await userManager.CreateAsync(applicationUser, registerAdminDto.Password);
        if (!createAdminResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createAdminResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating Admin",
                title: "Error creating Admin",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(applicationUser, Roles.Admin);
        if (!addRoleResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createAdminResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating Admin",
                title: "Error creating Admin",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        await applicationDbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest([Roles.Admin], null, registerAdminDto.AdminId)
        );
        await identityDbContext.SaveChangesAsync();
        return Ok(admin.ToFullInfoAdminDto(accessTokens.AccessToken, accessTokens.RefreshToken, Lang));
    }

    [HttpPost("Admin/Login")]
    public async Task<ActionResult<FullInfoAdminDto>> LoginAdmin(LoginAdminDto loginAdminDto)
    {
        Admin? admin = await applicationDbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Gmail == loginAdminDto.Gmail && a.MajorId == loginAdminDto.MajorId);
        if (admin is null)
        {
            return Problem(
                detail: "Admin not found",
                title: "Admin not found",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        ApplicationUser? applicationUser =
            await userManager.Users.FirstOrDefaultAsync(u => u.AdminId == admin.Id);
        if (applicationUser is null)
        {
            return Problem(
                detail: "Admin not Registered",
                title: "Admin not Registered",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        if (!await userManager.CheckPasswordAsync(applicationUser, loginAdminDto.Password))
        {
            return Problem(
                detail: "Wrong password",
                title: "Wrong password",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        IList<string> roles = await userManager.GetRolesAsync(applicationUser);
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(roles, null, admin.Id)
        );
        RefreshToken? refreshToken = await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.UserId == applicationUser.Id);
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
        return Ok(admin.ToFullInfoAdminDto(accessTokens.AccessToken, accessTokens.RefreshToken, Lang));
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
            new TokenRequest(roles, refreshToken.User.StudentId, refreshToken.User.AdminId, refreshToken.User.SuperAdminId, refreshToken.User.ProfessorId)
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

    private static bool IsAdminCodeValid(Admin admin, int code)
    {
        if (admin.OneTimeCodeCreatedDate is null ||
            admin.OneTimeCodeExpirationInMinutes is null || admin.OneTimeCode is null)
        {
            return false;
        }

        if (admin.OneTimeCode != code)
        {
            return false;
        }

        if (admin.OneTimeCodeCreatedDate!.Value.AddMinutes(admin.OneTimeCodeExpirationInMinutes!
                .Value) < DateTime.UtcNow)
        {
            return false;
        }

        return true;
    }
}
