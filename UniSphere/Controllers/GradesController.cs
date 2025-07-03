using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Grades;
using UniSphere.Api.Extensions;
using UniSphere.Api.Helpers;

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
            return Unauthorized(new { Message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
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
            return NotFound(new { Message = BilingualErrorMessages.GetNoGradesFoundMessage(Lang) });
        }

        var gradeDto = await dbContext.SubjectStudentLinks
            .Where(s =>
                s.StudentId == studentId
                && s.MidtermGrade != null
                && s.FinalGrade != null
            )
            .Include(ss=>ss.Subject)
            .Select(GradesQueries.ProjectToDto(Lang)).ToListAsync();

        return Ok(new GradesCollection
        {
            NumberOfFailedSubjects = collectionInfo.numberOfFailedSubjects,
            NumberOfPassedSubjects = collectionInfo.numberOfPassedSubjects,
            Average = collectionInfo.average ?? 0,
            Grades = gradeDto,
        });
    }

    [HttpPost("AssignGradesToSubject")]
    public async Task<IActionResult> AssignGradesToSubject([FromBody] DTOs.Grades.AssignGradesDto dto)
    {
        if (dto.StudentGrades == null || dto.StudentGrades.Count == 0)
        {
            return BadRequest(new { Message = BilingualErrorMessages.GetStudentGradesEmptyMessage(Lang) });
        }

        if (!dto.SubjectId.HasValue)
        {
            return BadRequest(new { Message = BilingualErrorMessages.GetSubjectIdRequiredMessage(Lang) });
        }

        var subject = await dbContext.Subjects.FindAsync(dto.SubjectId.Value);
        if (subject == null)
        {
            return NotFound(new { Message = BilingualErrorMessages.GetSubjectNotFoundMessage(Lang) });
        }
        double passGrade = subject.PassGrade;

        var subjectLinks = await dbContext.SubjectStudentLinks
            .Where(link => link.SubjectId == dto.SubjectId.Value && dto.StudentGrades.Select(sg => sg.StudentId).Contains(link.StudentId))
            .ToListAsync();

        var notFoundStudents = dto.StudentGrades
            .Where(sg => !subjectLinks.Any(link => link.StudentId == sg.StudentId))
            .Select(sg => sg.StudentId)
            .ToList();

        if (notFoundStudents.Any())
        {
            var studentIdsString = string.Join(", ", notFoundStudents);
            return NotFound(new { Message = BilingualErrorMessages.GetStudentsNotEnrolledMessage(Lang, studentIdsString) });
        }

        foreach (var sg in dto.StudentGrades)
        {
            var link = subjectLinks.FirstOrDefault(l => l.StudentId == sg.StudentId);
            if (link == null)
            {
                continue;
            }

            link.MidtermGrade = sg.MidTermGrade;
            link.FinalGrade = sg.FinalGrade;
            double totalGrade = (sg.MidTermGrade ?? 0) + (sg.FinalGrade ?? 0);
            if (totalGrade >= passGrade)
            {
                link.IsPassed = true;
                link.IsCurrentlyEnrolled = false;
            }
            // else: keep IsPassed and IsCurrentlyEnrolled as is
        }

        await dbContext.SaveChangesAsync();
        return Ok(new { 
            Success = true, 
            Message = BilingualErrorMessages.GetSuccessMessage(Lang) 
        });
    }
}
