using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Grades;
using UniSphere.Api.Extensions;

namespace UniSphere.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]

public class GradesController(ApplicationDbContext dbContext) : BaseController
{
    [HttpGet("GetMyGrades")]
    public async Task<ActionResult<GradesCollection>> GetMyGrades()
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            return Unauthorized();
        }

        var collectionInfo = await dbContext.SubjectStudentLinks
            .Where(s => s.StudentId == studentId
                        &&
                        s.MidtermGrade != null &&
                        s.FinalGrade != null)
            
            .GroupBy(s => 1)
            .Select(ss => new
                {
                    numberOfPassedSubjects = ss.Count(s => s.IsPassed),
                    numberOfFailedSubjects = ss.Count(s => !s.IsPassed),
                    average = ss.Average(s => s.MidtermGrade + s.FinalGrade)
                }
            ).FirstOrDefaultAsync();

        if (collectionInfo is null)
        {
            return NotFound();
        }

        var gradeDto = await dbContext.SubjectStudentLinks
            .Where(s =>
                s.StudentId == studentId
                && s.MidtermGrade != null
                && s.FinalGrade != null
            )
            .Include(ss=>ss.Subject)
            .Select(GradesQueries.ProjectToDto(Lang)).ToListAsync();


        // .SumAsync(s => s.TotalGrade);


        return Ok(new GradesCollection
        {
            NumberOfFailedSubjects = collectionInfo.numberOfFailedSubjects,
            NumberOfPassedSubjects = collectionInfo.numberOfPassedSubjects,
            Average = collectionInfo.average ?? 0,
            Grades = gradeDto,
        });
    }
}
