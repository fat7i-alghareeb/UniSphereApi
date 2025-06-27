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
[Produces("application/json")]
[Route("api/[controller]")]
public class AdminController(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider,
    IAuthService authService,
    IStorageService storageService
) : BaseController
{
    [HttpPost("AssignOneTimeCodeToStudent")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignOneTimeCodeToStudent([FromBody] AssignOneTimeCodeRequestDto dto)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized();
        }

        var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
        if (admin is null)
        {
            return Unauthorized();
        }

        if (dto.TargetRole != AssignOneTimeCodeTargetRole.Student)
        {
            return BadRequest("Admin can only assign one-time codes to students.");
        }

        if (dto.StudentId is null)
        {
            return BadRequest("StudentId is required.");
        }

        var student = await dbContext.StudentCredentials
            .Include(sc => sc.Major)
            .FirstOrDefaultAsync(sc => sc.Id == dto.StudentId);

        if (student is null || student.MajorId != admin.MajorId)
        {
            return Forbid();
        }

        int code = Random.Shared.Next(100_000, 1_000_000); // 6-digit code
        int expiration = dto.ExpirationInMinutes ?? 10;
        DateTime now = DateTime.UtcNow;

        student.OneTimeCode = code;
        student.OneTimeCodeCreatedDate = now;
        student.OneTimeCodeExpirationInMinutes = expiration;

        await dbContext.SaveChangesAsync();
        return Ok(new
        {
            message = "One-time code assigned successfully to student.",
            code,
            expiresAt = now.AddMinutes(expiration)
        });
    }

    [HttpPost("UploadProfileImage")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UploadProfileImage(IFormFile image)
    {
        try
        {
            var adminId = HttpContext.User.GetAdminId();
            if (adminId is null)
            {
                return Unauthorized();
            }

            var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
            if (admin is null)
            {
                return NotFound("Admin not found");
            }

            if (image == null || image.Length == 0)
            {
                return BadRequest("No image file provided");
            }

            // Validate image file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid image format. Allowed formats: jpg, jpeg, png, gif, bmp, webp");
            }

            // Validate file size (max 5MB for profile images)
            if (image.Length > 5 * 1024 * 1024)
            {
                return BadRequest("Image file size must be less than 5MB");
            }

            // Save the image using LocalStorageService
            var imageUrl = await storageService.SaveFileAsync(image, "admin-profiles");

            // Update the admin's image URL
            admin.Image = imageUrl;
            await dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile image uploaded successfully",
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
            return NotFound();
        }

        if (!await authService.ValidateOneTimeCodeAsync(admin, checkOneTimeCodeDto.Code))
        {
            return Problem(
                detail: "Code is not valid",
                title: "Code is not valid",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        return Ok(admin.ToSimpleAdminDto());
    }

    [HttpPost("Auth/Register")]
    [AllowAnonymous]
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
            return NotFound();
        }

        var applicationUser = new ApplicationUser
        {
            UserName = admin.Gmail,
            Email = admin.Gmail,
            EmailConfirmed = true,
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
    public async Task<ActionResult<FullInfoAdminDto>> Login(LoginAdminDto loginAdminDto)
    {
        // Find user by email
        var applicationUser = await userManager.FindByEmailAsync(loginAdminDto.Gmail);
        if (applicationUser is null)
        {
            return Problem(
                detail: "Admin not found",
                title: "Admin not found",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        // Check if user has AdminId
        if (applicationUser.AdminId is null)
        {
            return Problem(
                detail: "User is not an admin",
                title: "User is not an admin",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        Admin? admin = await dbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Id == applicationUser.AdminId);
        if (admin is null)
        {
            return Problem(
                detail: "Admin not found",
                title: "Admin not found",
                statusCode: StatusCodes.Status404NotFound
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
        
        await authService.CreateOrUpdateRefreshTokenAsync(applicationUser, accessTokens.RefreshToken);
        
        return Ok(admin.ToFullInfoAdminDto(accessTokens.AccessToken, accessTokens.RefreshToken, Roles.Admin));
    }
} 