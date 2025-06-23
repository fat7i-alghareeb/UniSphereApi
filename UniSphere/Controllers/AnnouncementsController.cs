using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Announcements;
using UniSphere.Api.Entities;
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
            .Select(AnnouncementsQueries.ProjectToStudentAnnouncementsDto(Lang))
            .ToListAsync();

        // Get announcements for subjects the student is enrolled in from other years
        var subjectAnnouncements = await dbContext.MajorAnnouncements
            .Where(a => studentSubjectsIdsForNotTheCurrentYear.Contains(a.SubjectId))
            .Select(AnnouncementsQueries.ProjectToStudentAnnouncementsDto(Lang))
            .ToListAsync();

        // Combine both lists
        var allAnnouncements = majorAnnouncements.Concat(subjectAnnouncements).ToList();
        
        return Ok(new StudentAnnouncementsCollectionDto
        {
            Announcements =   allAnnouncements
        });
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
            .Select(AnnouncementsQueries.ProjectToFacultyAnnouncementsDto(Lang))
            .ToListAsync();
        return Ok(new FacultyAnnouncementsCollectionDto
        {
            Announcements = announcements,
        });
    }
    // [HttpGet("GetFacultyAnnouncementById")]
    // public async Task<ActionResult<FacultyAnnouncementsDto>> GetFacultyAnnouncementById([Required] Guid announcementId)
    // {
    //     var studentId = HttpContext.User.GetStudentId();
    //     if (studentId is null)
    //     {
    //         return Unauthorized();
    //     }

    //     var announcement = await dbContext.FacultyAnnouncements
    //         .Where(a => a.Id == announcementId)
    //         .Select(AnnouncementsQueries.ProjectToFacultyAnnouncementsDto(Lang))
    //         .FirstOrDefaultAsync();
    //     return Ok(announcement);
    // }

    // Admin Announcement Creation Endpoints
    [HttpPost("CreateFacultyAnnouncement")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<FacultyAnnouncementsDto>> CreateFacultyAnnouncement(CreateFacultyAnnouncementDto createDto)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized();
        }

        // Get the admin and their major (and thus their faculty)
        var admin = await dbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Id == adminId);
        if (admin is null)
        {
            return Unauthorized();
        }
        var facultyId = admin.Major.FacultyId;

        var facultyAnnouncement = new FacultyAnnouncement
        {
            Id = Guid.NewGuid(),
            FacultyId = facultyId,
            Title = new MultilingualText { En = createDto.TitleEn, Ar = createDto.TitleAr },
            Content = new MultilingualText { En = createDto.ContentEn, Ar = createDto.ContentAr },
            CreatedAt = DateTime.UtcNow
        };

        dbContext.FacultyAnnouncements.Add(facultyAnnouncement);
        await dbContext.SaveChangesAsync();

        return Ok(facultyAnnouncement.ToFacultyAnnouncementsDto(Lang));
    }

    [HttpPost("CreateMajorAnnouncement")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StudentAnnouncementsDto>> CreateMajorAnnouncement(CreateMajorAnnouncementDto createDto)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized();
        }

        // Verify admin has access to this major
        var admin = await dbContext.Admins
            .FirstOrDefaultAsync(a => a.Id == adminId);

        if (admin is null || admin.MajorId != createDto.MajorId)
        {
            return Forbid();
        }

        // Verify subject belongs to the major
        var subject = await dbContext.Subjects
            .FirstOrDefaultAsync(s => s.Id == createDto.SubjectId && s.MajorId == createDto.MajorId);

        if (subject is null)
        {
            return BadRequest("Subject does not belong to the specified major");
        }

        var majorAnnouncement = new MajorAnnouncement
        {
            Id = Guid.NewGuid(),
            MajorId = createDto.MajorId,
            SubjectId = createDto.SubjectId,
            Year = createDto.Year,
            Title = new MultilingualText { En = createDto.TitleEn, Ar = createDto.TitleAr },
            Content = new MultilingualText { En = createDto.ContentEn, Ar = createDto.ContentAr },
            CreatedAt = DateTime.UtcNow
        };

        dbContext.MajorAnnouncements.Add(majorAnnouncement);
        await dbContext.SaveChangesAsync();

        return Ok(majorAnnouncement.ToStudentAnnouncementsDto(Lang));
    }
}
