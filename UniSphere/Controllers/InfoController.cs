using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Announcements;
using UniSphere.Api.DTOs.Info;
using UniSphere.Api.DTOs.Statistics;
using UniSphere.Api.DTOs.Subjects;
using UniSphere.Api.Extensions;
using UniSphere.Api.Helpers;

namespace UniSphere.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class InfoController(ApplicationDbContext dbContext) : BaseController
{
    [HttpGet("GetFaculties")]
    [AllowAnonymous]
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
    [AllowAnonymous]
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

    [HttpGet("SuperAdmin/GetMyFacultyMajors")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<MajorsCollectionDto>> GetSuperAdminFacultyMajors()
    {
        var superAdminId = HttpContext.User.GetSuperAdminId();
        if (superAdminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        // Get the faculty ID for the super admin
        var facultyId = await dbContext.SuperAdmins
            .Where(sa => sa.Id == superAdminId)
            .Select(sa => sa.FacultyId)
            .FirstOrDefaultAsync();

        if (facultyId == Guid.Empty)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        // Get majors for the super admin's faculty
        var majors = await dbContext.Majors
            .Where(m => m.FacultyId == facultyId)
            .Select(InfoQueries.ProjectToMajorNameDto(Lang))
            .ToListAsync();

        return Ok(new MajorsCollectionDto
        {
            Majors = majors
        });
    }

    [HttpGet("GetHomePageInfo")]
    [AllowAnonymous]
    public async Task<ActionResult<HomeDto>> GetHomePageInfo()
    {
        
        
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
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
            return NotFound(new { message = BilingualErrorMessages.GetNotFoundMessage(Lang) });
        }
        var studentInfo = await dbContext.StudentCredentials
     .Where(sc => sc.Id == studentId)
     .Include(sc => sc.Major)
     .ThenInclude(m => m.Faculty)
     .Select(sc => new
     {
         sc.Major.FacultyId
     })
     .Distinct()
     .FirstOrDefaultAsync();

        if (studentInfo is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetNotFoundMessage(Lang) });
        }

        var announcements = await dbContext.FacultyAnnouncements
        .Where(fa => fa.FacultyId == studentInfo.FacultyId)
        .OrderByDescending(fa => fa.CreatedAt)
        .Take(10)
        .Select(AnnouncementsQueries.ProjectToTop10FacultyAnnouncementsDto(Lang))
        .ToListAsync();
        var daysToTheFinal = await dbContext.Faculties
        .Where(f => f.Id == studentInfo.FacultyId)
        .Select(f => f.DaysToTheFinale)
        .FirstOrDefaultAsync();

        return Ok(new HomeDto
        {
            Announcements = announcements,
            DaysToTheFinal = daysToTheFinal,
            Statistics = statistics
        });
    }

    [HttpGet("Admin/MyMajorSubjects")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<SubjectNameIdDto>>> GetAdminMajorSubjects([Required] int year)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var majorId = await dbContext.Admins
            .Where(a => a.Id == adminId)
            .Select(a => a.MajorId)
            .FirstOrDefaultAsync();

        if (majorId == Guid.Empty)
        {
            return NotFound(new { message = BilingualErrorMessages.GetNotFoundMessage(Lang) });
        }

        var subjects = await dbContext.Subjects
            .Where(s => s.MajorId == majorId && s.Year == year)
            .Select(s => new SubjectNameIdDto
            {
                Id = s.Id,
                Name = s.Name.GetTranslatedString(Lang)
            })
            .ToListAsync();

        return Ok(subjects);
    }

}

