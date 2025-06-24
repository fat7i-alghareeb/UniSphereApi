using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Professors;
using UniSphere.Api.Entities;
using UniSphere.Api.Extensions;

namespace UniSphere.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ProfessorsController(ApplicationDbContext dbContext) : BaseController
{
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
} 