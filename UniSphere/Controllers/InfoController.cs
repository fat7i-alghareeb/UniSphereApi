using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Info;

namespace UniSphere.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Authorize]
[Route("api/[controller]")]
public class InfoController(ApplicationDbContext dbContext) : BaseController
{
    [HttpGet("GetFaculties")]
    public async Task<ActionResult<FacultiesCollectionDto>> GetFaculties()
    {
        var faculties = await dbContext.Faculties
            .Select(InfoQueries.ProjectToFacultyNameDto(Lang))
            .ToListAsync();

        return Ok(new FacultiesCollectionDto
            {
                Factories = faculties
            }
        );
    }

    [HttpGet("GetMajors")]
    public async Task<ActionResult<MajorsCollectionDto>> GetMajors([Required] Guid facultyId)
    {
        var majors = await dbContext.Majors
            .Where(m => m.FacultyId == facultyId)
            .Select(InfoQueries.ProjectToMajorNameDto(Lang))
            .ToListAsync();

        return Ok(
            new MajorsCollectionDto
            {
                Majors = majors
            }
        );
    }
}
