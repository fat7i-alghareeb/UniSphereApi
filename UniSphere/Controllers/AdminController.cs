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
using static UniSphere.Api.Helpers.OneTimeCodeHelper;

namespace UniSphere.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class AdminController(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider,
    IAuthService authService,
    IProfileImageService profileImageService
) : BaseController
{
    [HttpPost("AssignOneTimeCodeToStudent")]
    [Authorize(Roles = "Admin")]
    /// <summary>
    /// Assigns a one-time code to a student in the admin's major. Only Admins can assign codes to students in their own major.
    /// </summary>
    public async Task<IActionResult> AssignOneTimeCodeToStudent([FromBody] AssignOneTimeCodeRequestDto dto)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Problem(statusCode: 401, title: "Unauthorized", detail: BilingualErrorMessages.GetUnauthorizedMessage(Lang));
        }
        var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
        if (admin is null)
        {
            return Problem(statusCode: 401, title: "Unauthorized", detail: BilingualErrorMessages.GetUnauthorizedMessage(Lang));
        }
        if (dto.TargetRole != AssignOneTimeCodeTargetRole.Student)
        {
            return Problem(statusCode: 400, title: "Bad Request", detail: Lang == Languages.En ? "Admin can only assign one-time codes to students." : "يمكن للمسؤول تعيين رموز لمرة واحدة للطلاب فقط.");
        }
        var student = await dbContext.StudentCredentials
            .Include(sc => sc.Major)
            .FirstOrDefaultAsync(sc => sc.Id == dto.StudentId);
        if (student is null || student.MajorId != admin.MajorId)
        {
            return Problem(statusCode: 403, title: "Forbidden", detail: BilingualErrorMessages.GetNoAccessToSubjectMessage(Lang));
        }
        int code = dto.OneTimeCode ?? 1234 ; // Use provided code or generate
        int expiration = dto.ExpirationInMinutes ?? 10;
        AssignOneTimeCode(student, code, expiration);
        await dbContext.SaveChangesAsync();
        return Ok(new
        {
            message = Lang == Languages.En ? "One-time code assigned successfully to student." : "تم تعيين رمز لمرة واحدة للطالب بنجاح.",
            code,
            expiresAt = student.OneTimeCodeExpirationInMinutes.HasValue ? DateTime.UtcNow.AddMinutes(student.OneTimeCodeExpirationInMinutes.Value) : (DateTime?)null
        });
    }

    [HttpPost("UploadProfileImage")]
    [Authorize(Roles = "Admin")]
    /// <summary>
    /// Uploads a profile image for the current admin. Validates and stores the image.
    /// </summary>
    public async Task<IActionResult> UploadProfileImage(IFormFile image)
    {
        try
        {
            var adminId = HttpContext.User.GetAdminId();
            if (adminId is null)
            {
                return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
            }

            var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
            if (admin is null)
            {
                return NotFound(new { message = BilingualErrorMessages.GetStudentNotFoundMessage(Lang) });
            }

            var imageUrl = await profileImageService.UploadProfileImageAsync(image, "admin-profiles");
            admin.Image = imageUrl;
            await dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = Lang == Languages.En ? "Profile image uploaded successfully" : "تم رفع صورة الملف الشخصي بنجاح",
                imageUrl,
                fileName = image.FileName,
                fileSize = image.Length
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // Authentication Endpoints
    [HttpPost("Auth/CheckOneTimeCode")]
    [AllowAnonymous]
    public async Task<ActionResult<SimpleAdminDto>> CheckOneTimeCode(AdminCheckOneTimeCodeDto checkOneTimeCodeDto)
    {
        var admin = await dbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Gmail == checkOneTimeCodeDto.Gmail && a.MajorId == checkOneTimeCodeDto.MajorId);
        if (admin is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetStudentNotFoundMessage(Lang) });
        }

        if (!await authService.ValidateOneTimeCodeAsync(admin, checkOneTimeCodeDto.Code))
        {
            return Problem(
                detail: Lang == Languages.En ? "Code is not valid" : "الرمز غير صالح",
                title: Lang == Languages.En ? "Code is not valid" : "الرمز غير صالح",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        return Ok(admin.ToSimpleAdminDto());
    }

    [HttpPost("Auth/Register")]
    [AllowAnonymous]
    /// <summary>
    /// Registers a new admin user and links to the admin entity. Handles transaction and role assignment.
    /// </summary>
    public async Task<ActionResult<FullInfoAdminDto>> Register(RegisterAdminDto registerAdminDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        dbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await dbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());
        Admin? admin = await dbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Id == registerAdminDto.AdminId);
        if (admin is null)
        {
            return Problem(statusCode: 404, title: "Not Found", detail: BilingualErrorMessages.GetStudentNotFoundMessage(Lang));
        }
        var applicationUser = new ApplicationUser
        {
            UserName = admin.Gmail,
            Email = admin.Gmail,
            EmailConfirmed = true,
            AdminId = registerAdminDto.AdminId,
        };
        IdentityResult createAdminResult = await userManager.CreateAsync(applicationUser, registerAdminDto.Password);
        if (!createAdminResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createAdminResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };
            return Problem(
                detail: Lang == Languages.En ? "Error creating Admin" : "حدث خطأ أثناء إنشاء المسؤول",
                title: Lang == Languages.En ? "Error creating Admin" : "حدث خطأ أثناء إنشاء المسؤول",
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
                detail: Lang == Languages.En ? "Error creating Admin" : "حدث خطأ أثناء إنشاء المسؤول",
                title: Lang == Languages.En ? "Error creating Admin" : "حدث خطأ أثناء إنشاء المسؤول",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        admin.IdentityId = applicationUser.Id;
        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest([Roles.Admin], null, registerAdminDto.AdminId)
        );
        await identityDbContext.SaveChangesAsync();
        return Ok(admin.ToFullInfoAdminDto(accessTokens.AccessToken, accessTokens.RefreshToken, Roles.Admin));
    }

    [HttpPost("Auth/Login")]
    [AllowAnonymous]
    /// <summary>
    /// Authenticates an admin and returns full info with tokens.
    /// </summary>
    public async Task<ActionResult<FullInfoAdminDto>> Login(LoginAdminDto loginAdminDto)
    {
        // Find user by email
        var applicationUser = await userManager.FindByEmailAsync(loginAdminDto.Gmail);
        if (applicationUser is null)
        {
            return Problem(
                detail: Lang == Languages.En ? "Admin not found" : "لم يتم العثور على المسؤول",
                title: Lang == Languages.En ? "Admin not found" : "لم يتم العثور على المسؤول",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        // Check if user has AdminId
        if (applicationUser.AdminId is null)
        {
            return Problem(
                detail: Lang == Languages.En ? "User is not an admin" : "المستخدم ليس مسؤولاً",
                title: Lang == Languages.En ? "User is not an admin" : "المستخدم ليس مسؤولاً",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        Admin? admin = await dbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Id == applicationUser.AdminId);
        if (admin is null)
        {
            return Problem(
                detail: Lang == Languages.En ? "Admin not found" : "لم يتم العثور على المسؤول",
                title: Lang == Languages.En ? "Admin not found" : "لم يتم العثور على المسؤول",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        if (!await userManager.CheckPasswordAsync(applicationUser, loginAdminDto.Password))
        {
            return Problem(
                detail: Lang == Languages.En ? "Wrong password" : "كلمة المرور غير صحيحة",
                title: Lang == Languages.En ? "Wrong password" : "كلمة المرور غير صحيحة",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        IList<string> roles = await userManager.GetRolesAsync(applicationUser);
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(roles, null, admin.Id)
        );
        
        await authService.CreateOrUpdateRefreshTokenAsync(applicationUser, accessTokens.RefreshToken);
        
        return Ok(admin.ToFullInfoAdminDto(accessTokens.AccessToken, accessTokens.RefreshToken, Roles.Admin));
    }
} 
