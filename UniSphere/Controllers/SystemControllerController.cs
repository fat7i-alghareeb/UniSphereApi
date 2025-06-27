using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
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
[Authorize(Roles = "SystemController")]
[Route("api/[controller]")]
public class SystemControllerController(
    ApplicationDbContext applicationDbContext,
    UserManager<ApplicationUser> userManager,
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider,
    IAuthService authService,
    IStorageService storageService
) : BaseController
{
    [HttpPost("UploadProfileImage")]
    [Authorize(Roles = "SystemController")]
    public async Task<IActionResult> UploadProfileImage(IFormFile image)
    {
        try
        {
            var systemControllerId = HttpContext.User.GetSystemControllerId();
            if (systemControllerId is null)
            {
                return Unauthorized();
            }

            var systemController = await applicationDbContext.SystemControllers.FirstOrDefaultAsync(sc => sc.Id == systemControllerId);
            if (systemController is null)
            {
                return NotFound("SystemController not found");
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
            var imageUrl = await storageService.SaveFileAsync(image, "systemcontroller-profiles");

            // Update the systemController's image URL
            systemController.Image = imageUrl;
            await applicationDbContext.SaveChangesAsync();

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

    // SuperAdmin Management Endpoints
    [HttpPost("SuperAdmin/Create")]
    public async Task<ActionResult<BaseSuperAdminDto>> CreateSuperAdmin(CreateSuperAdminDto createDto)
    {
        // Check if faculty exists
        var faculty = await applicationDbContext.Faculties.FirstOrDefaultAsync(f => f.Id == createDto.FacultyId);
        if (faculty is null)
        {
            return BadRequest("Faculty not found");
        }

        // Check if gmail already exists
        var existingSuperAdmin = await applicationDbContext.SuperAdmins.FirstOrDefaultAsync(sa => sa.Gmail == createDto.Gmail);
        if (existingSuperAdmin is not null)
        {
            return BadRequest("A SuperAdmin with this Gmail already exists");
        }

        var superAdmin = new SuperAdmin
        {
            Id = Guid.NewGuid(),
            FirstName = new MultilingualText { En = createDto.FirstNameEn, Ar = createDto.FirstNameAr },
            LastName = new MultilingualText { En = createDto.LastNameEn, Ar = createDto.LastNameAr },
            Gmail = createDto.Gmail,
            FacultyId = createDto.FacultyId
        };

        applicationDbContext.SuperAdmins.Add(superAdmin);
        await applicationDbContext.SaveChangesAsync();

        return Ok(superAdmin.ToBaseSuperAdminDto());
    }

    [HttpDelete("SuperAdmin/Remove/{superAdminId:guid}")]
    public async Task<ActionResult> RemoveSuperAdmin(Guid superAdminId)
    {
        var superAdmin = await applicationDbContext.SuperAdmins.FirstOrDefaultAsync(sa => sa.Id == superAdminId);
        if (superAdmin is null)
        {
            return NotFound("SuperAdmin not found");
        }

        // Check if SuperAdmin has any associated ApplicationUser
        var applicationUser = await userManager.Users.FirstOrDefaultAsync(u => u.SuperAdminId == superAdminId);
        if (applicationUser is not null)
        {
            return BadRequest("Cannot remove SuperAdmin with registered account. Delete the account first.");
        }

        applicationDbContext.SuperAdmins.Remove(superAdmin);
        await applicationDbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("SuperAdmin/AssignOneTimeCode")]
    public async Task<ActionResult> AssignSuperAdminOneTimeCode(AssignSuperAdminOneTimeCodeDto dto)
    {
        var superAdmin = await applicationDbContext.SuperAdmins.FirstOrDefaultAsync(sa => sa.Id == dto.SuperAdminId);
        if (superAdmin is null)
        {
            return NotFound("SuperAdmin not found");
        }

        int code = Random.Shared.Next(100_000, 1_000_000); // 6-digit code
        int expiration = dto.ExpirationInMinutes ?? 10;
        DateTime now = DateTime.UtcNow;

        superAdmin.OneTimeCode = code;
        superAdmin.OneTimeCodeCreatedDate = now;
        superAdmin.OneTimeCodeExpirationInMinutes = expiration;

        await applicationDbContext.SaveChangesAsync();

        return Ok(new
        {
            message = "One-time code assigned successfully.",
            code,
            expiresAt = now.AddMinutes(expiration)
        });
    }

    // EnrollmentStatus Management Endpoints
    [HttpPost("EnrollmentStatus/Create")]
    public async Task<ActionResult<EnrollmentStatus>> CreateEnrollmentStatus(CreateEnrollmentStatusDto createDto)
    {
        var enrollmentStatus = new EnrollmentStatus
        {
            Id = Guid.NewGuid(),
            Name = new MultilingualText { En = createDto.NameEn, Ar = createDto.NameAr }
        };

        applicationDbContext.EnrollmentStatuses.Add(enrollmentStatus);
        await applicationDbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEnrollmentStatus), new { id = enrollmentStatus.Id }, enrollmentStatus);
    }

    [HttpGet("EnrollmentStatus/{id:guid}")]
    public async Task<ActionResult<EnrollmentStatus>> GetEnrollmentStatus(Guid id)
    {
        var enrollmentStatus = await applicationDbContext.EnrollmentStatuses.FirstOrDefaultAsync(es => es.Id == id);
        if (enrollmentStatus is null)
        {
            return NotFound();
        }

        return Ok(enrollmentStatus);
    }

    [HttpGet("EnrollmentStatus")]
    public async Task<ActionResult<List<EnrollmentStatus>>> GetAllEnrollmentStatuses()
    {
        var enrollmentStatuses = await applicationDbContext.EnrollmentStatuses.ToListAsync();
        return Ok(enrollmentStatuses);
    }

    [HttpPatch("EnrollmentStatus/{id:guid}")]
    public async Task<ActionResult<EnrollmentStatus>> UpdateEnrollmentStatus(Guid id, [FromBody] JsonPatchDocument<UpdateEnrollmentStatusDto> patchDoc)
    {
        if (patchDoc is null)
        {
            return BadRequest();
        }

        var enrollmentStatus = await applicationDbContext.EnrollmentStatuses.FirstOrDefaultAsync(es => es.Id == id);
        if (enrollmentStatus is null)
        {
            return NotFound();
        }

        var updateDto = new UpdateEnrollmentStatusDto();
        patchDoc.ApplyTo(updateDto, ModelState);
        if (!TryValidateModel(updateDto))
        {
            return ValidationProblem(ModelState);
        }

        // Apply updates
        if (updateDto.NameEn is not null)
        {
            enrollmentStatus.Name.En = updateDto.NameEn;
        }
        if (updateDto.NameAr is not null)
        {
            enrollmentStatus.Name.Ar = updateDto.NameAr;
        }

        await applicationDbContext.SaveChangesAsync();
        return Ok(enrollmentStatus);
    }

    [HttpDelete("EnrollmentStatus/{id:guid}")]
    public async Task<ActionResult> RemoveEnrollmentStatus(Guid id)
    {
        var enrollmentStatus = await applicationDbContext.EnrollmentStatuses
            .Include(es => es.StudentCredentials)
            .FirstOrDefaultAsync(es => es.Id == id);
        
        if (enrollmentStatus is null)
        {
            return NotFound();
        }

        // Check if any students are using this enrollment status
        if (enrollmentStatus.StudentCredentials.Any())
        {
            return BadRequest("Cannot remove enrollment status that is being used by students");
        }

        applicationDbContext.EnrollmentStatuses.Remove(enrollmentStatus);
        await applicationDbContext.SaveChangesAsync();

        return NoContent();
    }

    // Authentication Endpoints
    [HttpPost("Auth/Login")]
    [AllowAnonymous]
    public async Task<ActionResult<FullInfoSystemControllerDto>> Login(LoginSystemControllerDto loginSystemControllerDto)
    {
        // Find user by email
        var applicationUser = await userManager.FindByEmailAsync(loginSystemControllerDto.Gmail);
        if (applicationUser is null)
        {
            return Problem(
                detail: "SystemController not found",
                title: "SystemController not found",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        // Check if user has SystemControllerId
        if (applicationUser.SystemControllerId is null)
        {
            return Problem(
                detail: "User is not a system controller",
                title: "User is not a system controller",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        SystemController? systemController = await applicationDbContext.SystemControllers
            .FirstOrDefaultAsync(sc => sc.Id == applicationUser.SystemControllerId);
        if (systemController is null)
        {
            return Problem(
                detail: "SystemController not found",
                title: "SystemController not found",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        if (!await userManager.CheckPasswordAsync(applicationUser, loginSystemControllerDto.Password))
        {
            return Problem(
                detail: "Wrong password",
                title: "Wrong password",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        IList<string> roles = await userManager.GetRolesAsync(applicationUser);
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(roles, null, null, null, null, systemController.Id)
        );
        
        await authService.CreateOrUpdateRefreshTokenAsync(applicationUser, accessTokens.RefreshToken);
        
        return Ok(systemController.ToFullInfoSystemControllerDto(accessTokens.AccessToken, accessTokens.RefreshToken, Roles.SystemController));
    }

    [HttpPost("Auth/Register")]
    [AllowAnonymous]
    public async Task<ActionResult<FullInfoSystemControllerDto>> Register(RegisterSystemControllerDto registerSystemControllerDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        SystemController? systemController = await applicationDbContext.SystemControllers
            .FirstOrDefaultAsync(sc => sc.Id == registerSystemControllerDto.SystemControllerId);
        if (systemController is null)
        {
            return NotFound();
        }

        var applicationUser = new ApplicationUser
        {
            UserName = systemController.Gmail,
            Email = systemController.Gmail,
            EmailConfirmed = true,
            SystemControllerId = registerSystemControllerDto.SystemControllerId,
        };
        if (registerSystemControllerDto.Password != registerSystemControllerDto.ConfirmPassword)
        {
            return BadRequest("Password and ConfirmPassword must be the same");
        }

        IdentityResult createSystemControllerResult = await userManager.CreateAsync(applicationUser, registerSystemControllerDto.Password);
        if (!createSystemControllerResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createSystemControllerResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating SystemController",
                title: "Error creating SystemController",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(applicationUser, Roles.SystemController);
        if (!addRoleResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createSystemControllerResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating SystemController",
                title: "Error creating SystemController",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        systemController.IdentityId = applicationUser.Id;
        await applicationDbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest([Roles.SystemController], null, null, null, null, registerSystemControllerDto.SystemControllerId)
        );
        await identityDbContext.SaveChangesAsync();
        return Ok(systemController.ToFullInfoSystemControllerDto(accessTokens.AccessToken, accessTokens.RefreshToken, Roles.SystemController));
    }
} 