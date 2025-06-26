using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.Entities;
using UniSphere.Api.Extensions;
using UniSphere.Api.Services;

namespace UniSphere.Api.Controllers;

[ApiController]
[Authorize(Roles = "SuperAdmin")]
[Route("api/[controller]")]
public class SuperAdminController(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider,
    IAuthService authService
) : BaseController
{
    [HttpGet("GetMe")]
    public async Task<ActionResult<BaseSuperAdminDto>> GetMe()
    {
        var superAdminId = HttpContext.User.GetSuperAdminId();

        if (superAdminId is null)
        {
            return Unauthorized();
        }

        SuperAdmin superAdmin = await dbContext.SuperAdmins
            .Include(sa => sa.Faculty)
            .FirstOrDefaultAsync(sa => sa.Id == superAdminId);
        if (superAdmin is null)
        {
            return Unauthorized();
        }
        return Ok(superAdmin.ToBaseSuperAdminDto());
    }

    [HttpPost("AssignOneTimeCode")]
    public async Task<IActionResult> AssignOneTimeCode([FromBody] AssignOneTimeCodeRequestDto dto)
    {
        var superAdminId = HttpContext.User.GetSuperAdminId();
        if (superAdminId is null)
        {
            return Unauthorized();
        }
        var superAdmin = await dbContext.SuperAdmins.FirstOrDefaultAsync(sa => sa.Id == superAdminId);
        if (superAdmin is null)
        {
            return Unauthorized();
        }
        var facultyId = superAdmin.FacultyId;
        int code = Random.Shared.Next(100_000, 1_000_000); // 6-digit code
        int expiration = dto.ExpirationInMinutes ?? 10;
        DateTime now = DateTime.UtcNow;
        switch (dto.TargetRole)
        {
            case AssignOneTimeCodeTargetRole.Admin:
                if (dto.AdminId is null){
                    return BadRequest("AdminId is required.");
                }
                var admin = await dbContext.Admins.Include(a => a.Major).FirstOrDefaultAsync(a => a.Id == dto.AdminId);
                if (admin is null || admin.Major.FacultyId != facultyId){
                    return Forbid();
                }
                admin.OneTimeCode = code;
                admin.OneTimeCodeCreatedDate = now;
                admin.OneTimeCodeExpirationInMinutes = expiration;
                break;
            case AssignOneTimeCodeTargetRole.Professor:
                if (dto.ProfessorId is null){
                    return BadRequest("ProfessorId is required.");
                }
                var professorLink = await dbContext.ProfessorFacultyLinks.FirstOrDefaultAsync(pfl => pfl.ProfessorId == dto.ProfessorId && pfl.FacultyId == facultyId);
                if (professorLink is null){
                    return Forbid();
                }
                var professor = await dbContext.Professors.FirstOrDefaultAsync(p => p.Id == dto.ProfessorId);
                if (professor is null){
                    return NotFound("Professor not found.");
                }
                professor.OneTimeCode = code;
                professor.OneTimeCodeCreatedDate = now;
                professor.OneTimeCodeExpirationInMinutes = expiration;
                break;
            case AssignOneTimeCodeTargetRole.Student:
                    if (dto.StudentId is null){
                    return BadRequest("StudentId is required.");
                }
                var student = await dbContext.StudentCredentials.Include(sc => sc.Major).FirstOrDefaultAsync(sc => sc.Id == dto.StudentId);
                if (student is null || student.Major.FacultyId != facultyId){
                    return Forbid();
                }
                student.OneTimeCode = code;
                student.OneTimeCodeCreatedDate = now;
                student.OneTimeCodeExpirationInMinutes = expiration;
                break;
            default:
                return BadRequest("Invalid target role.");
        }
        await dbContext.SaveChangesAsync();
        return Ok(new
        {
            message = "One-time code assigned successfully.",
            code,
            expiresAt = now.AddMinutes(expiration)
        });
    }

    // Authentication Endpoints
    [HttpPost("Auth/CheckOneTimeCode")]
    [AllowAnonymous]
    public async Task<ActionResult<SimpleSuperAdminDto>> CheckOneTimeCode(SuperAdminCheckOneTimeCodeDto checkOneTimeCodeDto)
    {
        var superAdmin = await dbContext.SuperAdmins
            .Include(sa => sa.Faculty)
            .FirstOrDefaultAsync(sa => sa.Gmail == checkOneTimeCodeDto.Gmail && sa.FacultyId == checkOneTimeCodeDto.FacultyId);
        if (superAdmin is null)
        {
            return NotFound();
        }

        if (!await authService.ValidateOneTimeCodeAsync(superAdmin, checkOneTimeCodeDto.Code))
        {
            return Problem(
                detail: "Code is not valid",
                title: "Code is not valid",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        return Ok(superAdmin.ToSimpleSuperAdminDto());
    }

    [HttpPost("Auth/Register")]
    [AllowAnonymous]
    public async Task<ActionResult<FullInfoSuperAdminDto>> Register(RegisterSuperAdminDto registerSuperAdminDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        dbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await dbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        SuperAdmin? superAdmin = await dbContext.SuperAdmins
            .Include(sa => sa.Faculty)
            .FirstOrDefaultAsync(sa => sa.Id == registerSuperAdminDto.SuperAdminId);
        if (superAdmin is null)
        {
            return NotFound();
        }

        var applicationUser = new ApplicationUser
        {
            UserName = registerSuperAdminDto.SuperAdminId.ToString(),
            SuperAdminId = registerSuperAdminDto.SuperAdminId,
        };
        if (registerSuperAdminDto.Password != registerSuperAdminDto.ConfirmPassword)
        {
            return BadRequest("Password and ConfirmPassword must be the same");
        }

        IdentityResult createSuperAdminResult = await userManager.CreateAsync(applicationUser, registerSuperAdminDto.Password);
        if (!createSuperAdminResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createSuperAdminResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating SuperAdmin",
                title: "Error creating SuperAdmin",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(applicationUser, Roles.SuperAdmin);
        if (!addRoleResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createSuperAdminResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating SuperAdmin",
                title: "Error creating SuperAdmin",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        superAdmin.IdentityId = applicationUser.Id;
        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest([Roles.SuperAdmin], null, null, registerSuperAdminDto.SuperAdminId)
        );
        await identityDbContext.SaveChangesAsync();
        return Ok(superAdmin.ToFullInfoSuperAdminDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }

    [HttpPost("Auth/Login")]
    [AllowAnonymous]
    public async Task<ActionResult<FullInfoSuperAdminDto>> Login(LoginSuperAdminDto loginSuperAdminDto)
    {
        SuperAdmin? superAdmin = await dbContext.SuperAdmins
            .Include(sa => sa.Faculty)
            .FirstOrDefaultAsync(sa => sa.Gmail == loginSuperAdminDto.Gmail && sa.FacultyId == loginSuperAdminDto.FacultyId);
        if (superAdmin is null)
        {
            return Problem(
                detail: "SuperAdmin not found",
                title: "SuperAdmin not found",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        ApplicationUser? applicationUser =
            await userManager.Users.FirstOrDefaultAsync(u => u.SuperAdminId == superAdmin.Id);
        if (applicationUser is null)
        {
            return Problem(
                detail: "SuperAdmin not Registered",
                title: "SuperAdmin not Registered",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        if (!await userManager.CheckPasswordAsync(applicationUser, loginSuperAdminDto.Password))
        {
            return Problem(
                detail: "Wrong password",
                title: "Wrong password",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        IList<string> roles = await userManager.GetRolesAsync(applicationUser);
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(roles, null, null, superAdmin.Id)
        );
        
        await authService.CreateOrUpdateRefreshTokenAsync(applicationUser, accessTokens.RefreshToken);
        
        return Ok(superAdmin.ToFullInfoSuperAdminDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }
} 
