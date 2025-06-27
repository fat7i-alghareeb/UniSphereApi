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
                return Unauthorized();
            }

            var professor = await dbContext.Professors.FirstOrDefaultAsync(p => p.Id == professorId);
            if (professor is null)
            {
                return NotFound("Professor not found");
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
            var imageUrl = await storageService.SaveFileAsync(image, "professor-profiles");

            // Update the professor's image URL
            professor.Image = imageUrl;
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

    [HttpPost("AddProfessorToFaculty")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> AddProfessorToFaculty(AddProfessorToFacultyDto dto)
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
        
        var professor = await dbContext.Professors.FirstOrDefaultAsync(p => p.Id == dto.ProfessorId);
        if (professor is null)
        {
            return NotFound("Professor not found");
        }
        
        // Check if professor is already linked to this faculty
        var existingLink = await dbContext.ProfessorFacultyLinks
            .FirstOrDefaultAsync(pfl => pfl.ProfessorId == dto.ProfessorId && pfl.FacultyId == superAdmin.FacultyId);
        
        if (existingLink != null)
        {
            return BadRequest("Professor is already linked to this faculty");
        }
        
        // Create new link
        var professorFacultyLink = new ProfessorFacultyLink
        {
            ProfessorId = dto.ProfessorId,
            FacultyId = superAdmin.FacultyId
        };
        
        dbContext.ProfessorFacultyLinks.Add(professorFacultyLink);
        await dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("RemoveProfessorFromFaculty")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> RemoveProfessorFromFaculty(RemoveProfessorFromFacultyDto dto)
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
        
        var professorFacultyLink = await dbContext.ProfessorFacultyLinks
            .FirstOrDefaultAsync(pfl => pfl.ProfessorId == dto.ProfessorId && pfl.FacultyId == superAdmin.FacultyId);
        
        if (professorFacultyLink is null)
        {
            return NotFound("Professor not found in your faculty");
        }
        
        dbContext.ProfessorFacultyLinks.Remove(professorFacultyLink);
        await dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("AssignProfessorToSubject")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> AssignProfessorToSubject([FromBody] AssignProfessorToSubjectDto dto)
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
        
        // Verify professor belongs to the superAdmin's faculty
        var professorFacultyLink = await dbContext.ProfessorFacultyLinks
            .FirstOrDefaultAsync(pfl => pfl.ProfessorId == dto.ProfessorId && pfl.FacultyId == superAdmin.FacultyId);
        
        if (professorFacultyLink is null)
        {
            return Forbid("Professor does not belong to your faculty");
        }
        
        // Verify subject belongs to the superAdmin's faculty
        var subject = await dbContext.Subjects
            .Include(s => s.Major)
            .FirstOrDefaultAsync(s => s.Id == dto.SubjectId);
        
        if (subject is null || subject.Major.FacultyId != superAdmin.FacultyId)
        {
            return Forbid("Subject does not belong to your faculty");
        }
        
        // Check if professor is already assigned to this subject
        var existingAssignment = await dbContext.SubjectProfessorLinks
            .FirstOrDefaultAsync(spl => spl.ProfessorId == dto.ProfessorId && spl.SubjectId == dto.SubjectId);
        
        if (existingAssignment != null)
        {
            return BadRequest("Professor is already assigned to this subject");
        }
        
        // Create new assignment
        var subjectProfessorLink = new SubjectProfessorLink
        {
            ProfessorId = dto.ProfessorId,
            SubjectId = dto.SubjectId
        };
        
        dbContext.SubjectProfessorLinks.Add(subjectProfessorLink);
        await dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("RemoveProfessorFromSubject")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> RemoveProfessorFromSubject([FromBody] AssignProfessorToSubjectDto dto)
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
        
        // Verify subject belongs to the superAdmin's faculty
        var subject = await dbContext.Subjects
            .Include(s => s.Major)
            .FirstOrDefaultAsync(s => s.Id == dto.SubjectId);
        
        if (subject is null || subject.Major.FacultyId != superAdmin.FacultyId)
        {
            return Forbid("Subject does not belong to your faculty");
        }
        
        var subjectProfessorLink = await dbContext.SubjectProfessorLinks
            .FirstOrDefaultAsync(spl => spl.ProfessorId == dto.ProfessorId && spl.SubjectId == dto.SubjectId);
        
        if (subjectProfessorLink is null)
        {
            return NotFound("Professor is not assigned to this subject");
        }
        
        dbContext.SubjectProfessorLinks.Remove(subjectProfessorLink);
        await dbContext.SaveChangesAsync();
        return Ok();
    }

    [HttpPatch("EditProfessor/{professorId:guid}")]
    [Authorize(Roles = "SuperAdmin,Professor")]
    public async Task<IActionResult> EditProfessor(Guid professorId, [FromBody] JsonPatchDocument<ProfessorUpdateDto> patchDoc)
    {

        
        var superAdminId = HttpContext.User.GetSuperAdminId();
        var currentProfessorId = HttpContext.User.GetProfessorId();
        
        if (superAdminId is null && (currentProfessorId is null || currentProfessorId != professorId))
        {
            return Forbid();
        }
        
        var professor = await dbContext.Professors.FirstOrDefaultAsync(p => p.Id == professorId);
        if (professor is null)
        {
            return NotFound();
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
        return Ok();
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
            return NotFound(
                    "Professor not found."
                );
        }

        if (!await authService.ValidateOneTimeCodeAsync(professor, checkOneTimeCodeDto.Code))
        {
            return Problem(
                detail: "Code is not valid",
                title: "Code is not valid",
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
            return NotFound();
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
            return BadRequest("Password and ConfirmPassword must be the same");
        }

        IdentityResult createProfessorResult = await userManager.CreateAsync(applicationUser, registerProfessorDto.Password);
        if (!createProfessorResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createProfessorResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating Professor",
                title: "Error creating Professor",
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
                detail: "Error creating Professor",
                title: "Error creating Professor",
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
                detail: "Professor not found",
                title: "Professor not found",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        // Check if user has ProfessorId
        if (applicationUser.ProfessorId is null)
        {
            return Problem(
                detail: "User is not a professor",
                title: "User is not a professor",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        Professor? professor = await dbContext.Professors
            .FirstOrDefaultAsync(p => p.Id == applicationUser.ProfessorId);
        if (professor is null)
        {
            return Problem(
                detail: "Professor not found",
                title: "Professor not found",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        if (!await userManager.CheckPasswordAsync(applicationUser, loginProfessorDto.Password))
        {
            return Problem(
                detail: "Wrong password",
                title: "Wrong password",
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
