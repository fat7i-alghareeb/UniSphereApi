using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.DTOs.Professors;
using UniSphere.Api.Entities;
using UniSphere.Api.Extensions;
using UniSphere.Api.Services;
using UniSphere.Api.Helpers;

namespace UniSphere.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProfessorsController(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider,
    IAuthService authService,
    IStorageService storageService
) : BaseController
{
    [HttpPost("UploadProfileImage")]
    [Authorize(Roles = "Professor")]
    public async Task<IActionResult> UploadProfileImage(IFormFile image)
    {
        try
        {
            var professorId = HttpContext.User.GetProfessorId();
            if (professorId is null)
            {
                return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
            }

            var professor = await dbContext.Professors.FirstOrDefaultAsync(p => p.Id == professorId);
            if (professor is null)
            {
                return NotFound(new { message = BilingualErrorMessages.GetNotFoundMessage(Lang) });
            }

            if (image == null || image.Length == 0)
            {
                return BadRequest(new { message = Lang == Languages.En ? "No image file provided" : "لم يتم توفير ملف صورة" });
            }

            // Validate image file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { message = Lang == Languages.En ? "Invalid image format. Allowed formats: jpg, jpeg, png, gif, bmp, webp" : "تنسيق الصورة غير صالح. التنسيقات المسموح بها: jpg, jpeg, png, gif, bmp, webp" });
            }

            // Validate file size (max 5MB for profile images)
            if (image.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { message = Lang == Languages.En ? "Image file size must be less than 5MB" : "يجب أن يكون حجم ملف الصورة أقل من 5 ميجابايت" });
            }

            // Save the image using LocalStorageService
            var imageUrl = await storageService.SaveFileAsync(image, "professor-profiles");

            // Update the professor's image URL
            professor.Image = imageUrl;
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

    [HttpPost("AddProfessorToFaculty")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> AddProfessorToFaculty(AddProfessorToFacultyDto dto)
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
        
        var professor = await dbContext.Professors.FirstOrDefaultAsync(p => p.Id == dto.ProfessorId);
        if (professor is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetNotFoundMessage(Lang) });
        }
        
        // Check if professor is already linked to this faculty
        var existingLink = await dbContext.ProfessorFacultyLinks
            .FirstOrDefaultAsync(pfl => pfl.ProfessorId == dto.ProfessorId && pfl.FacultyId == superAdmin.FacultyId);
        
        if (existingLink != null)
        {
            return BadRequest(new { message = Lang == Languages.En ? "Professor is already linked to this faculty" : "الأستاذ مرتبط بالفعل بهذه الكلية" });
        }
        
        // Create new link
        var professorFacultyLink = new ProfessorFacultyLink
        {
            ProfessorId = dto.ProfessorId,
            FacultyId = superAdmin.FacultyId
        };
        
        dbContext.ProfessorFacultyLinks.Add(professorFacultyLink);
        await dbContext.SaveChangesAsync();
        return Ok(new { message = Lang == Languages.En ? "Professor added to faculty successfully" : "تمت إضافة الأستاذ إلى الكلية بنجاح" });
    }

    [HttpDelete("RemoveProfessorFromFaculty")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> RemoveProfessorFromFaculty(RemoveProfessorFromFacultyDto dto)
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
        
        var professorFacultyLink = await dbContext.ProfessorFacultyLinks
            .FirstOrDefaultAsync(pfl => pfl.ProfessorId == dto.ProfessorId && pfl.FacultyId == superAdmin.FacultyId);
        
        if (professorFacultyLink is null)
        {
            return NotFound(new { message = Lang == Languages.En ? "Professor not found in your faculty" : "الأستاذ غير موجود في كليتك" });
        }
        
        dbContext.ProfessorFacultyLinks.Remove(professorFacultyLink);
        await dbContext.SaveChangesAsync();
        return Ok(new { message = Lang == Languages.En ? "Professor removed from faculty successfully" : "تمت إزالة الأستاذ من الكلية بنجاح" });
    }

    [HttpPost("AssignProfessorToSubject")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> AssignProfessorToSubject([FromBody] AssignProfessorToSubjectDto dto)
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
        
        // Verify professor belongs to the superAdmin's faculty
        var professorFacultyLink = await dbContext.ProfessorFacultyLinks
            .FirstOrDefaultAsync(pfl => pfl.ProfessorId == dto.ProfessorId && pfl.FacultyId == superAdmin.FacultyId);
        
        if (professorFacultyLink is null)
        {
            return Forbid(BilingualErrorMessages.GetNoAccessToSubjectMessage(Lang));
        }
        
        // Verify subject belongs to the superAdmin's faculty
        var subject = await dbContext.Subjects
            .Include(s => s.Major)
            .FirstOrDefaultAsync(s => s.Id == dto.SubjectId);
        
        if (subject is null || subject.Major.FacultyId != superAdmin.FacultyId)
        {
            return Forbid(BilingualErrorMessages.GetNoAccessToSubjectMessage(Lang));
        }
        
        // Check if professor is already assigned to this subject
        var existingAssignment = await dbContext.SubjectProfessorLinks
            .FirstOrDefaultAsync(spl => spl.ProfessorId == dto.ProfessorId && spl.SubjectId == dto.SubjectId);
        
        if (existingAssignment != null)
        {
            return BadRequest(new { message = Lang == Languages.En ? "Professor is already assigned to this subject" : "الأستاذ معين بالفعل لهذه المادة" });
        }
        
        // Create new assignment
        var subjectProfessorLink = new SubjectProfessorLink
        {
            ProfessorId = dto.ProfessorId,
            SubjectId = dto.SubjectId
        };
        
        dbContext.SubjectProfessorLinks.Add(subjectProfessorLink);
        await dbContext.SaveChangesAsync();
        return Ok(new { message = Lang == Languages.En ? "Professor assigned to subject successfully" : "تم تعيين الأستاذ للمادة بنجاح" });
    }

    [HttpDelete("RemoveProfessorFromSubject")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> RemoveProfessorFromSubject([FromBody] AssignProfessorToSubjectDto dto)
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
        
        // Verify subject belongs to the superAdmin's faculty
        var subject = await dbContext.Subjects
            .Include(s => s.Major)
            .FirstOrDefaultAsync(s => s.Id == dto.SubjectId);
        
        if (subject is null || subject.Major.FacultyId != superAdmin.FacultyId)
        {
            return Forbid(BilingualErrorMessages.GetNoAccessToSubjectMessage(Lang));
        }
        
        var subjectProfessorLink = await dbContext.SubjectProfessorLinks
            .FirstOrDefaultAsync(spl => spl.ProfessorId == dto.ProfessorId && spl.SubjectId == dto.SubjectId);
        
        if (subjectProfessorLink is null)
        {
            return NotFound(new { message = Lang == Languages.En ? "Professor is not assigned to this subject" : "الأستاذ غير معين لهذه المادة" });
        }
        
        dbContext.SubjectProfessorLinks.Remove(subjectProfessorLink);
        await dbContext.SaveChangesAsync();
        return Ok(new { message = Lang == Languages.En ? "Professor removed from subject successfully" : "تمت إزالة الأستاذ من المادة بنجاح" });
    }

    [HttpPatch("EditProfessor/{professorId:guid}")]
    [Authorize(Roles = "SuperAdmin,Professor")]
    public async Task<IActionResult> EditProfessor(Guid professorId, [FromBody] JsonPatchDocument<ProfessorUpdateDto> patchDoc)
    {
        var superAdminId = HttpContext.User.GetSuperAdminId();
        var currentProfessorId = HttpContext.User.GetProfessorId();
        if (superAdminId is null && (currentProfessorId is null || currentProfessorId != professorId))
        {
            return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
        }
        var professor = await dbContext.Professors.FirstOrDefaultAsync(p => p.Id == professorId);
        if (professor is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetNotFoundMessage(Lang) });
        }
        // Use mappings
        var updateDto = professor.ToUpdateDto();
        patchDoc.ApplyTo(updateDto, ModelState);
        if (!TryValidateModel(updateDto))
        {
            return ValidationProblem(ModelState);
        }
        professor.PatchFromDto(updateDto);
        await dbContext.SaveChangesAsync();
        return Ok(new { message = Lang == Languages.En ? "Professor updated successfully" : "تم تحديث بيانات الأستاذ بنجاح" });
    }

    // Authentication Endpoints
    [HttpPost("Auth/CheckOneTimeCode")]
    [AllowAnonymous]
    public async Task<ActionResult<SimpleProfessorDto>> CheckOneTimeCode(ProfessorCheckOneTimeCodeDto checkOneTimeCodeDto)
    {
        var professor = await dbContext.Professors
            .FirstOrDefaultAsync(p => p.Gmail == checkOneTimeCodeDto.Gmail);
        if (professor is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetNotFoundMessage(Lang) });
        }

        if (!await authService.ValidateOneTimeCodeAsync(professor, checkOneTimeCodeDto.Code))
        {
            return Problem(
                detail: Lang == Languages.En ? "Code is not valid" : "الرمز غير صالح",
                title: Lang == Languages.En ? "Code is not valid" : "الرمز غير صالح",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        return Ok(professor.ToSimpleProfessorDto());
    }

    [HttpPost("Auth/Register")]
    [AllowAnonymous]
    public async Task<ActionResult<FullInfoProfessorDto>> Register(RegisterProfessorDto registerProfessorDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        dbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await dbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        Professor? professor = await dbContext.Professors
            .FirstOrDefaultAsync(p => p.Id == registerProfessorDto.ProfessorId);
        if (professor is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetNotFoundMessage(Lang) });
        }

        var applicationUser = new ApplicationUser
        {
            UserName = professor.Gmail,
            Email = professor.Gmail,
            EmailConfirmed = true,
            ProfessorId = registerProfessorDto.ProfessorId,
        };
        if (registerProfessorDto.Password != registerProfessorDto.ConfirmPassword)
        {
            return BadRequest(new { message = Lang == Languages.En ? "Password and ConfirmPassword must be the same" : "يجب أن تكون كلمة المرور وتأكيد كلمة المرور متطابقتين" });
        }

        IdentityResult createProfessorResult = await userManager.CreateAsync(applicationUser, registerProfessorDto.Password);
        if (!createProfessorResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createProfessorResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: Lang == Languages.En ? "Error creating Professor" : "حدث خطأ أثناء إنشاء الأستاذ",
                title: Lang == Languages.En ? "Error creating Professor" : "حدث خطأ أثناء إنشاء الأستاذ",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(applicationUser, Roles.Professor);
        if (!addRoleResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createProfessorResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: Lang == Languages.En ? "Error creating Professor" : "حدث خطأ أثناء إنشاء الأستاذ",
                title: Lang == Languages.En ? "Error creating Professor" : "حدث خطأ أثناء إنشاء الأستاذ",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        professor.IdentityId = applicationUser.Id;
        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest([Roles.Professor], null, null, null, registerProfessorDto.ProfessorId)
        );
        await identityDbContext.SaveChangesAsync();
        return Ok(professor.ToFullInfoProfessorDto(accessTokens.AccessToken, accessTokens.RefreshToken, Roles.Professor));
    }

    [HttpPost("Auth/Login")]
    [AllowAnonymous]
    public async Task<ActionResult<FullInfoProfessorDto>> Login(LoginProfessorDto loginProfessorDto)
    {
        // Find user by email
        var applicationUser = await userManager.FindByEmailAsync(loginProfessorDto.Gmail);
        if (applicationUser is null)
        {
            return Problem(
                detail: Lang == Languages.En ? "Professor not found" : "لم يتم العثور على الأستاذ",
                title: Lang == Languages.En ? "Professor not found" : "لم يتم العثور على الأستاذ",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        // Check if user has ProfessorId
        if (applicationUser.ProfessorId is null)
        {
            return Problem(
                detail: Lang == Languages.En ? "User is not a professor" : "المستخدم ليس أستاذاً",
                title: Lang == Languages.En ? "User is not a professor" : "المستخدم ليس أستاذاً",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        Professor? professor = await dbContext.Professors
            .FirstOrDefaultAsync(p => p.Id == applicationUser.ProfessorId);
        if (professor is null)
        {
            return Problem(
                detail: Lang == Languages.En ? "Professor not found" : "لم يتم العثور على الأستاذ",
                title: Lang == Languages.En ? "Professor not found" : "لم يتم العثور على الأستاذ",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        if (!await userManager.CheckPasswordAsync(applicationUser, loginProfessorDto.Password))
        {
            return Problem(
                detail: Lang == Languages.En ? "Wrong password" : "كلمة المرور غير صحيحة",
                title: Lang == Languages.En ? "Wrong password" : "كلمة المرور غير صحيحة",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        IList<string> roles = await userManager.GetRolesAsync(applicationUser);
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(roles, null, null, null, professor.Id)
        );
        
        await authService.CreateOrUpdateRefreshTokenAsync(applicationUser, accessTokens.RefreshToken);
        
        return Ok(professor.ToFullInfoProfessorDto(accessTokens.AccessToken, accessTokens.RefreshToken, Roles.Professor));
    }
} 
