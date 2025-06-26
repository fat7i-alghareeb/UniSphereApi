using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
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
    // Student Endpoints
    [HttpGet("Student/GetMyAnnouncements")]
    public async Task<ActionResult<StudentAnnouncementsCollectionDto>> GetMyAnnouncements()
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
            Announcements = allAnnouncements
        });
    }

    [HttpGet("Student/GetFacultyAnnouncements")]
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

    // Admin Endpoints
    [HttpGet("Admin/GetMyAnnouncements")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<StudentAnnouncementsCollectionDto>> GetAdminMyAnnouncements([FromQuery] int? year)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized();
        }

        var admin = await dbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Id == adminId);
        if (admin is null)
        {
            return Unauthorized();
        }

        var query = dbContext.MajorAnnouncements
            .Where(a => a.MajorId == admin.MajorId);

        if (year.HasValue)
        {
            query = query.Where(a => a.Year == year.Value);
        }

        var announcements = await query
            .Select(AnnouncementsQueries.ProjectToStudentAnnouncementsDto(Lang))
            .ToListAsync();

        return Ok(new StudentAnnouncementsCollectionDto
        {
            Announcements = announcements
        });
    }

    [HttpGet("Admin/GetFacultyAnnouncements")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<FacultyAnnouncementsCollectionDto>> GetAdminFacultyAnnouncements()
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized();
        }

        var admin = await dbContext.Admins
            .Include(a => a.Major)
            .ThenInclude(m => m.Faculty)
            .FirstOrDefaultAsync(a => a.Id == adminId);
        if (admin is null)
        {
            return Unauthorized();
        }

        var announcements = await dbContext.FacultyAnnouncements
            .Where(a => a.FacultyId == admin.Major.FacultyId)
            .Select(AnnouncementsQueries.ProjectToFacultyAnnouncementsDto(Lang))
            .ToListAsync();

        return Ok(new FacultyAnnouncementsCollectionDto
        {
            Announcements = announcements,
        });
    }

    // PATCH and DELETE Endpoints
    [HttpPatch("FacultyAnnouncements/{id:guid}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<FacultyAnnouncementsDto>> UpdateFacultyAnnouncement(Guid id, JsonPatchDocument<FacultyAnnouncementsDto> patchDocument)
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

        var announcement = await dbContext.FacultyAnnouncements
            .FirstOrDefaultAsync(a => a.Id == id && a.FacultyId == superAdmin.FacultyId);
        if (announcement is null)
        {
            return NotFound();
        }

        var announcementDto = announcement.ToFacultyAnnouncementsDto(Lang);
        patchDocument.ApplyTo(announcementDto, ModelState);
        if (!TryValidateModel(announcementDto))
        {
            return ValidationProblem(ModelState);
        }

        // Update the entity from DTO
        announcement.UpdateFromDto(announcementDto);

        await dbContext.SaveChangesAsync();
        return Ok(announcement.ToFacultyAnnouncementsDto(Lang));
    }

    [HttpDelete("FacultyAnnouncements/{id:guid}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult> DeleteFacultyAnnouncement(Guid id)
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

        var announcement = await dbContext.FacultyAnnouncements
            .FirstOrDefaultAsync(a => a.Id == id && a.FacultyId == superAdmin.FacultyId);
        if (announcement is null)
        {
            return NotFound();
        }

        dbContext.FacultyAnnouncements.Remove(announcement);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPatch("MajorAnnouncements/{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<StudentAnnouncementsDto>> UpdateMajorAnnouncement(Guid id, JsonPatchDocument<StudentAnnouncementsDto> patchDocument)
    {
        var adminId = HttpContext.User.GetAdminId();
        var superAdminId = HttpContext.User.GetSuperAdminId();
        Guid? majorId = null;

        if (adminId is not null)
        {
            var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
            if (admin is null)
            {
                return Unauthorized();
            }
            majorId = admin.MajorId;
        }
        else if (superAdminId is not null)
        {
            var superAdmin = await dbContext.SuperAdmins.FirstOrDefaultAsync(sa => sa.Id == superAdminId);
            if (superAdmin is null)
            {
                return Unauthorized();
            }
            // For superAdmin, we need to check if the announcement belongs to their faculty
            var existingAnnouncement = await dbContext.MajorAnnouncements
                .Include(a => a.Major)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (existingAnnouncement is null || existingAnnouncement.Major.FacultyId != superAdmin.FacultyId)
            {
                return Forbid();
            }
        }
        else
        {
            return Unauthorized();
        }

        var query = dbContext.MajorAnnouncements.Where(a => a.Id == id);
        if (majorId.HasValue)
        {
            query = query.Where(a => a.MajorId == majorId.Value);
        }

        var announcement = await query.FirstOrDefaultAsync();
        if (announcement is null)
        {
            return NotFound();
        }

        var announcementDto = announcement.ToStudentAnnouncementsDto(Lang);
        patchDocument.ApplyTo(announcementDto, ModelState);
        if (!TryValidateModel(announcementDto))
        {
            return ValidationProblem(ModelState);
        }

        // Update the entity from DTO
        announcement.UpdateFromDto(announcementDto);

        await dbContext.SaveChangesAsync();
        return Ok(announcement.ToStudentAnnouncementsDto(Lang));
    }

    [HttpDelete("MajorAnnouncements/{id:guid}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult> DeleteMajorAnnouncement(Guid id)
    {
        var adminId = HttpContext.User.GetAdminId();
        var superAdminId = HttpContext.User.GetSuperAdminId();
        Guid? majorId = null;

        if (adminId is not null)
        {
            var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
            if (admin is null)
            {
                return Unauthorized();
            }
            majorId = admin.MajorId;
        }
        else if (superAdminId is not null)
        {
            var superAdmin = await dbContext.SuperAdmins.FirstOrDefaultAsync(sa => sa.Id == superAdminId);
            if (superAdmin is null)
            {
                return Unauthorized();
            }
            // For superAdmin, we need to check if the announcement belongs to their faculty
            var existingAnnouncement = await dbContext.MajorAnnouncements
                .Include(a => a.Major)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (existingAnnouncement is null || existingAnnouncement.Major.FacultyId != superAdmin.FacultyId)
            {
                return Forbid();
            }
        }
        else
        {
            return Unauthorized();
        }

        var query = dbContext.MajorAnnouncements.Where(a => a.Id == id);
        if (majorId.HasValue)
        {
            query = query.Where(a => a.MajorId == majorId.Value);
        }

        var announcement = await query.FirstOrDefaultAsync();
        if (announcement is null)
        {
            return NotFound();
        }

        dbContext.MajorAnnouncements.Remove(announcement);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    // Admin Announcement Creation Endpoints
    [HttpPost("CreateFacultyAnnouncement")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<FacultyAnnouncementsDto>> CreateFacultyAnnouncement(CreateFacultyAnnouncementDto createDto)
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

        var facultyAnnouncement = createDto.ToFacultyAnnouncement(superAdmin.FacultyId);

        dbContext.FacultyAnnouncements.Add(facultyAnnouncement);
        await dbContext.SaveChangesAsync();

        return Ok(facultyAnnouncement.ToFacultyAnnouncementsDto(Lang));
    }

    [HttpPost("CreateMajorAnnouncement")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<StudentAnnouncementsDto>> CreateMajorAnnouncement(CreateMajorAnnouncementDto createDto)
    {
        var adminId = HttpContext.User.GetAdminId();
        var superAdminId = HttpContext.User.GetSuperAdminId();
        Guid majorId;

        if (adminId is not null)
        {
            var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
            if (admin is null)
            {
                return Unauthorized();
            }
            majorId = admin.MajorId;
        }
        else if (superAdminId is not null)
        {
            if (createDto.MajorId is null)
            {
                return BadRequest("MajorId is required for super admin.");
            }
            var superAdmin = await dbContext.SuperAdmins.FirstOrDefaultAsync(sa => sa.Id == superAdminId);
            var major = await dbContext.Majors.FirstOrDefaultAsync(m => m.Id == createDto.MajorId);
            if (superAdmin is null || major is null || superAdmin.FacultyId != major.FacultyId)
            {
                return Forbid();
            }
            majorId = createDto.MajorId.Value;
        }
        else
        {
            return Unauthorized();
        }

        // Verify subject belongs to the selected major
        var subject = await dbContext.Subjects
            .FirstOrDefaultAsync(s => s.Id == createDto.SubjectId && s.MajorId == majorId);

        if (subject is null)
        {
            return BadRequest("Subject does not belong to the selected major");
        }

        var majorAnnouncement = createDto.ToMajorAnnouncement(majorId);

        dbContext.MajorAnnouncements.Add(majorAnnouncement);
        await dbContext.SaveChangesAsync();

        return Ok(majorAnnouncement.ToStudentAnnouncementsDto(Lang));
    }
}
