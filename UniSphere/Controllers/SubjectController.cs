using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Subjects;

namespace UniSphere.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public sealed class SubjectController(ApplicationDbContext dbContext) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<SubjectCollectionDto>> GetSubjects()
    {
        List<SubjectDto> subjects = await dbContext.Subjects.Select(SubjectQueries.ProjectToDto()).ToListAsync();
        var subjectCollectionDto = new SubjectCollectionDto { Subjects = subjects };
        return Ok(subjectCollectionDto);
    }
}
