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
    IAuthService authService
) : BaseController
{
    [HttpGet("GetMe")]
    [Authorize(Roles = "Professor")]
    public async Task<ActionResult<BaseProfessorDto>> GetMe()
    {
        var professorId = HttpContext.User.GetProfessorId();

        if (professorId is null)
        {
            return Unauthorized();
        }

        Professor professor = await dbContext.Professors
            .FirstOrDefaultAsync(p => p.Id == professorId);
        if (professor is null)
        {
            return Unauthorized();
        }
        return Ok(professor.ToBaseProfessorDto());
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

    [HttpPatch("EditProfessor/{professorId:guid}")]
    [Authorize(Roles = "SuperAdmin,Professor")]
    public async Task<IActionResult> EditProfessor(Guid professorId, [FromBody] JsonPatchDocument<ProfessorUpdateDto> patchDoc)
    {
        if (patchDoc is null)
        {
            return BadRequest();
        }
        
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
            return NotFound();
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
            UserName = registerProfessorDto.ProfessorId.ToString(),
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
        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest([Roles.Professor], null, null, null, registerProfessorDto.ProfessorId)
        );
        await identityDbContext.SaveChangesAsync();
        return Ok(professor.ToFullInfoProfessorDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }

    [HttpPost("Auth/Login")]
    [AllowAnonymous]
    public async Task<ActionResult<FullInfoProfessorDto>> Login(LoginProfessorDto loginProfessorDto)
    {
        Professor? professor = await dbContext.Professors
            .FirstOrDefaultAsync(p => p.Gmail == loginProfessorDto.Gmail);
        if (professor is null)
        {
            return Problem(
                detail: "Professor not found",
                title: "Professor not found",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        ApplicationUser? applicationUser =
            await userManager.Users.FirstOrDefaultAsync(u => u.ProfessorId == professor.Id);
        if (applicationUser is null)
        {
            return Problem(
                detail: "Professor not Registered",
                title: "Professor not Registered",
                statusCode: StatusCodes.Status401Unauthorized
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
        
        return Ok(professor.ToFullInfoProfessorDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }
} 