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
public class StudentController(ApplicationDbContext dbContext) : BaseController
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
        return Ok(studentCredential.ToBaseStudentDto());
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
            .Select(StatisticsQueries.ProjectToDto(average ?? 0))
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
            return BadRequest(new IsSuccessDto { IsSuccess = false, Message = e.Message });

        }
        return Ok(new IsSuccessDto { IsSuccess = true });
    }

    [HttpPost("AddStudent")]
    [Authorize(Roles = nameof(Roles.SuperAdmin) + "," + nameof(Roles.Admin))]
    public async Task<IActionResult> AddStudent([FromBody] CreateStudentDto dto)
    {
        var userRole = HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
        var superAdminId = HttpContext.User.GetSuperAdminId();
        var adminId = HttpContext.User.GetAdminId();

        if (userRole == Roles.SuperAdmin)
        {
            // SuperAdmin: can add to any major in their faculty
            if (superAdminId is null)
            {
                return Unauthorized();
            }
            var superAdmin = await dbContext.SuperAdmins.FirstOrDefaultAsync(sa => sa.Id == superAdminId);
            if (superAdmin is null)
            {
                return Unauthorized();
            }
            var major = await dbContext.Majors.FirstOrDefaultAsync(m => m.Id == dto.MajorId);
            if (major is null || major.FacultyId != superAdmin.FacultyId)
            {
                return Forbid();
            }
        }
        else if (userRole == Roles.Admin)
        {
            // Admin: can add only to their assigned major
            if (adminId is null)
            {
                return Unauthorized();
            }
            var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
            if (admin is null || admin.MajorId != dto.MajorId)
            {
                return Forbid();
            }
        }
        else
        {
            return Forbid();
        }

        // Check for duplicate student number in the same major
        var exists = await dbContext.StudentCredentials.AnyAsync(sc => sc.StudentNumber == dto.StudentNumber && sc.MajorId == dto.MajorId);
        if (exists)
        {
            return BadRequest("A student with this number already exists in the major.");
        }

        // Get a default EnrollmentStatusId (first available)
        var defaultEnrollmentStatus = await dbContext.EnrollmentStatuses.FirstOrDefaultAsync();
        if (defaultEnrollmentStatus is null)
        {
            return BadRequest("No enrollment status found in the system.");
        }

        var student = new StudentCredential
        {
            Id = Guid.NewGuid(),
            StudentNumber = dto.StudentNumber,
            MajorId = dto.MajorId,
            FirstName = new Entities.MultilingualText { En = dto.FirstNameEn, Ar = dto.FirstNameAr },
            LastName = new Entities.MultilingualText { En = dto.LastNameEn, Ar = dto.LastNameAr },
            Year = dto.Year,
            EnrollmentStatusId = defaultEnrollmentStatus.Id
        };
        dbContext.StudentCredentials.Add(student);
        await dbContext.SaveChangesAsync();
        return Ok(new { message = "Student added successfully.", studentId = student.Id });
    }
}
