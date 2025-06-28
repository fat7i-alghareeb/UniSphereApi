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
using UniSphere.Api.Helpers;

namespace UniSphere.Api.Controllers;

[ApiController]
[Authorize(Roles = "SuperAdmin")]
[Route("api/[controller]")]
public class SuperAdminController(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider,
    IAuthService authService,
    IStorageService storageService
) : BaseController
{
    [HttpPost("AssignOneTimeCode")]
    public async Task<IActionResult> AssignOneTimeCode([FromBody] AssignOneTimeCodeRequestDto dto)
    {
        var superAdminId = HttpContext.User.GetSuperAdminId();
        if (superAdminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var superAdmin = await dbContext.SuperAdmins.FirstOrDefaultAsync(sa => sa.Id == superAdminId);
        if (superAdmin is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        // Determine target entity based on role
        switch (dto.TargetRole)
        {
            case AssignOneTimeCodeTargetRole.Admin:
                if (dto.AdminId is null)
                {
                    return BadRequest(new { message = BilingualErrorMessages.GetBadRequestMessage(Lang) });
                }
                var admin = await dbContext.Admins
                    .Include(a => a.Major)
                    .FirstOrDefaultAsync(a => a.Id == dto.AdminId);
                if (admin is null || admin.Major.FacultyId != superAdmin.FacultyId)
                {
                    return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
                }
                break;

            case AssignOneTimeCodeTargetRole.Professor:
                if (dto.ProfessorId is null)
                {
                    return BadRequest(new { message = BilingualErrorMessages.GetBadRequestMessage(Lang) });
                }
                var professorFacultyLink = await dbContext.ProfessorFacultyLinks
                    .FirstOrDefaultAsync(pfl => pfl.ProfessorId == dto.ProfessorId && pfl.FacultyId == superAdmin.FacultyId);
                if (professorFacultyLink is null)
                {
                    return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
                }
                break;

            case AssignOneTimeCodeTargetRole.Student:
                if (dto.StudentId is null)
                {
                    return BadRequest(new { message = BilingualErrorMessages.GetBadRequestMessage(Lang) });
                }
                var student = await dbContext.StudentCredentials
                    .Include(sc => sc.Major)
                    .FirstOrDefaultAsync(sc => sc.Id == dto.StudentId);
                if (student is null || student.Major.FacultyId != superAdmin.FacultyId)
                {
                    return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
                }
                break;

            default:
                return BadRequest(new { message = BilingualErrorMessages.GetBadRequestMessage(Lang) });
        }

        int code = Random.Shared.Next(100_000, 1_000_000); // 6-digit code
        int expiration = dto.ExpirationInMinutes ?? 10;
        DateTime now = DateTime.UtcNow;

        // Assign the code to the appropriate entity
        switch (dto.TargetRole)
        {
            case AssignOneTimeCodeTargetRole.Admin:
                var adminToUpdate = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == dto.AdminId);
                if (adminToUpdate is not null)
                {
                    adminToUpdate.OneTimeCode = code;
                    adminToUpdate.OneTimeCodeCreatedDate = now;
                    adminToUpdate.OneTimeCodeExpirationInMinutes = expiration;
                }
                break;

            case AssignOneTimeCodeTargetRole.Professor:
                var professorToUpdate = await dbContext.Professors.FirstOrDefaultAsync(p => p.Id == dto.ProfessorId);
                if (professorToUpdate is not null)
                {
                    professorToUpdate.OneTimeCode = code;
                    professorToUpdate.OneTimeCodeCreatedDate = now;
                    professorToUpdate.OneTimeCodeExpirationInMinutes = expiration;
                }
                break;

            case AssignOneTimeCodeTargetRole.Student:
                var studentToUpdate = await dbContext.StudentCredentials.FirstOrDefaultAsync(sc => sc.Id == dto.StudentId);
                if (studentToUpdate is not null)
                {
                    studentToUpdate.OneTimeCode = code;
                    studentToUpdate.OneTimeCodeCreatedDate = now;
                    studentToUpdate.OneTimeCodeExpirationInMinutes = expiration;
                }
                break;
        }

        await dbContext.SaveChangesAsync();
        return Ok(new
        {
            message = Lang == Languages.En ? "One-time code assigned successfully." : "تم تعيين رمز لمرة واحدة بنجاح.",
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
            return NotFound(new { message = BilingualErrorMessages.GetNotFoundMessage(Lang) });
        }

        if (!await authService.ValidateOneTimeCodeAsync(superAdmin, checkOneTimeCodeDto.Code))
        {
            return BadRequest(new { message = BilingualErrorMessages.GetInvalidCodeMessage(Lang) });
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
            return NotFound(new { message = BilingualErrorMessages.GetNotFoundMessage(Lang) });
        }

        var applicationUser = new ApplicationUser
        {
            UserName = superAdmin.Gmail,
            Email = superAdmin.Gmail,
            EmailConfirmed = true,
            SuperAdminId = registerSuperAdminDto.SuperAdminId,
        };
        if (registerSuperAdminDto.Password != registerSuperAdminDto.ConfirmPassword)
        {
            return BadRequest(new { message = BilingualErrorMessages.GetPasswordMismatchMessage(Lang) });
        }

        IdentityResult createSuperAdminResult = await userManager.CreateAsync(applicationUser, registerSuperAdminDto.Password);
        if (!createSuperAdminResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createSuperAdminResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: BilingualErrorMessages.GetErrorCreatingSuperAdminMessage(Lang),
                title: BilingualErrorMessages.GetErrorCreatingSuperAdminMessage(Lang),
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
                detail: BilingualErrorMessages.GetErrorCreatingSuperAdminMessage(Lang),
                title: BilingualErrorMessages.GetErrorCreatingSuperAdminMessage(Lang),
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        superAdmin.IdentityId = applicationUser.Id;
        await dbContext.SaveChangesAsync();
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest([Roles.SuperAdmin], null, null, registerSuperAdminDto.SuperAdminId)
        );
        await authService.CreateOrUpdateRefreshTokenAsync(applicationUser, accessTokens.RefreshToken);

        await identityDbContext.SaveChangesAsync();
        
        await transaction.CommitAsync();
        return Ok(superAdmin.ToFullInfoSuperAdminDto(accessTokens.AccessToken, accessTokens.RefreshToken, Roles.SuperAdmin));
    }

    [HttpPost("Auth/Login")]
    [AllowAnonymous]
    public async Task<ActionResult<FullInfoSuperAdminDto>> Login(LoginSuperAdminDto loginSuperAdminDto)
    {
        // Find user by email
        var applicationUser = await userManager.FindByEmailAsync(loginSuperAdminDto.Gmail);
        if (applicationUser is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetSuperAdminNotFoundMessage(Lang) });
        }

        // Check if user has SuperAdminId
        if (applicationUser.SuperAdminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUserNotSuperAdminMessage(Lang) });
        }

        SuperAdmin? superAdmin = await dbContext.SuperAdmins
            .Include(sa => sa.Faculty)
            .FirstOrDefaultAsync(sa => sa.Id == applicationUser.SuperAdminId);
        if (superAdmin is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetSuperAdminNotFoundMessage(Lang) });
        }

        if (!await userManager.CheckPasswordAsync(applicationUser, loginSuperAdminDto.Password))
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetWrongPasswordMessage(Lang) });
        }

        IList<string> roles = await userManager.GetRolesAsync(applicationUser);
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(roles, null, null, superAdmin.Id)
        );
        
        await authService.CreateOrUpdateRefreshTokenAsync(applicationUser, accessTokens.RefreshToken);
        
        return Ok(superAdmin.ToFullInfoSuperAdminDto(accessTokens.AccessToken, accessTokens.RefreshToken, Roles.SuperAdmin));
    }

    [HttpPost("UploadProfileImage")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> UploadProfileImage(IFormFile image)
    {
        try
        {
            var superAdminId = HttpContext.User.GetSuperAdminId();
            if (superAdminId is null)
            {
                return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
            }

            var superAdmin = await dbContext.SuperAdmins.FirstOrDefaultAsync(sa => sa.Id == superAdminId);
            if (superAdmin is null)
            {
                return NotFound(new { message = BilingualErrorMessages.GetNotFoundMessage(Lang) });
            }

            if (image == null || image.Length == 0)
            {
                return BadRequest(new { message = BilingualErrorMessages.GetNoImageFileMessage(Lang) });
            }

            // Validate image file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { message = BilingualErrorMessages.GetInvalidImageFormatMessage(Lang) });
            }

            // Validate file size (max 5MB for profile images)
            if (image.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { message = BilingualErrorMessages.GetImageFileSizeTooLargeMessage(Lang) });
            }

            // Save the image using LocalStorageService
            var imageUrl = await storageService.SaveFileAsync(image, "superadmin-profiles");

            // Update the superAdmin's image URL
            superAdmin.Image = imageUrl;
            await dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = BilingualErrorMessages.GetProfileImageUploadedMessage(Lang),
                imageUrl,
                fileName = image.FileName,
                fileSize = image.Length
            });
        }
        catch (Exception)
        {
            return BadRequest(new { message = BilingualErrorMessages.GetFileUploadErrorMessage(Lang) });
        }
    }
} 
