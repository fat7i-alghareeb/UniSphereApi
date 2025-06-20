using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs;
using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.DTOs.Statistics;
using UniSphere.Api.Entities;
using UniSphere.Api.Extensions;

namespace UniSphere.Api.Controllers;
 
[ApiController]
[Produces("application/json")]
[Authorize]
[Route("api/[controller]")]
public class StudentController (ApplicationDbContext dbContext) : BaseController
{
    [HttpGet("GetMe")]
    public async Task<ActionResult<BaseStudentDto>> GetMe()
    {
        var studentId = HttpContext.User.GetStudentId();
     
        if (studentId is null)
        {
            return Unauthorized(); 
        }

        StudentCredential studentCredential = await dbContext.StudentCredentials
            .Include(sc => sc.EnrollmentStatus)
            .Include(sc => sc.Major)
            .FirstOrDefaultAsync(sc => sc.Id == studentId);
        if (studentCredential is null)
        {
            return Unauthorized();
        }
        return Ok(studentCredential.ToBaseStudentDto(Lang));
    }

    [HttpGet("GetMyStatistics")]
    public async Task<ActionResult<StatisticsDto>> GetMyStatistics()
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            return Unauthorized();
        }
        var average = await dbContext.SubjectStudentLinks
            .Where(s => s.StudentId == studentId
                        &&
                        s.MidtermGrade != null &&
                        s.FinalGrade != null)
            .AverageAsync(s => s.MidtermGrade + s.FinalGrade);

        var statistics = await dbContext.StudentStatistics
            .Where(ss => ss.StudentId == studentId)
            .Select(StatisticsQueries.ProjectToDto(average?? 0))
            .FirstOrDefaultAsync();
        if (statistics is null)
        {
            return NotFound();
        }
        return Ok(statistics);
    }

    [HttpPatch("AddAttendance")]

    public async Task<ActionResult<IsSuccessDto>> AddAttendance(double hours, int lectures)
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            return Unauthorized();
        }
        var studentStatistics = await dbContext.StudentStatistics
            .Where(ss => ss.StudentId == studentId)
            .FirstOrDefaultAsync();
        if (studentStatistics is null)
        {
            return NotFound(new IsSuccessDto
            {
                IsSuccess = false,
                Message = "Statistics not found"
            }
);
        }
        try
        {
            studentStatistics.NumberOfAttendanceHours += hours;
            studentStatistics.NumberOfAttendanceLectures += lectures;
            await dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            return BadRequest(new IsSuccessDto { IsSuccess = false , Message = e.Message });
            
        }
        return Ok(new IsSuccessDto { IsSuccess = true });        
    }

}
