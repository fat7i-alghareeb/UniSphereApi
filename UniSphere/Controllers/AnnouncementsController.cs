using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Announcements;
using UniSphere.Api.Extensions;

namespace UniSphere.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AnnouncementsController(ApplicationDbContext dbContext) : BaseController
{
    [HttpGet("GetStudentAnnouncements")]
    public async Task<ActionResult<StudentAnnouncementsCollectionDto>> GetStudentAnnouncements()
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            return Unauthorized();
        }

        // First get all needed data in one query
        var studentInfo = await dbContext.StudentCredentials
            .Where(sc => sc.Id == studentId)
            .Select(sc => new
            {
                sc.Year,
                sc.MajorId
            })
            .FirstOrDefaultAsync();

        if (studentInfo is null)
        {
            return Unauthorized();
        }

        var studentSubjectsIdsForNotTheCurrentYear = await dbContext.SubjectStudentLinks
            .Include(s => s.Subject)
            .Where(s => s.StudentId == studentId && s.IsCurrentlyEnrolled && s.Subject.Year != studentInfo.Year)
            .Select(s => s.Subject.Id)
            .ToListAsync();

        // Get all major announcements for the student's major and year
        var majorAnnouncements = await dbContext.MajorAnnouncements
            .Where(a => a.MajorId == studentInfo.MajorId && a.Year == studentInfo.Year)
            .Select(a => a.ToStudentAnnouncementsDto(Lang))
            .ToListAsync();

        // Get announcements for subjects the student is enrolled in from other years
        var subjectAnnouncements = await dbContext.MajorAnnouncements
            .Where(a => studentSubjectsIdsForNotTheCurrentYear.Contains(a.SubjectId))
            .Select(a => a.ToStudentAnnouncementsDto(Lang))
            .ToListAsync();

        // Combine both lists
        var allAnnouncements = majorAnnouncements.Concat(subjectAnnouncements).ToList();
        return Ok(allAnnouncements);
    }

    [HttpGet("GetFacultyAnnouncements")]
    public async Task<ActionResult<FacultyAnnouncementsCollectionDto>> GetFacultyAnnouncements()
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            return Unauthorized();
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
            return Unauthorized();
        }

        var announcements = await dbContext.FacultyAnnouncements
            .Where(a => a.FacultyId == studentInfo.FacultyId)
            .Select(a => a.ToFacultyAnnouncementsDto(Lang))
            .ToListAsync();
        return Ok(announcements);
    }
}
