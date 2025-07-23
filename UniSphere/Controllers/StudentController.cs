using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs;
using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.DTOs.Info;
using UniSphere.Api.DTOs.Statistics;
using UniSphere.Api.Entities;
using UniSphere.Api.Extensions;
using UniSphere.Api.Services;
using UniSphere.Api.Helpers;

namespace UniSphere.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Authorize]
[Route("api/[controller]")]
public class StudentController(ApplicationDbContext dbContext, IProfileImageService profileImageService) : BaseController
{
    [HttpGet("GetMyStatistics")]
    public async Task<ActionResult<StatisticsDto>> GetMyStatistics()
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
        return Ok(statistics);
    }

    [HttpPatch("AddAttendance")]
    public async Task<ActionResult<IsSuccessDto>> AddAttendance(double hours, int lectures)
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }
        var studentStatistics = await dbContext.StudentStatistics
            .Where(ss => ss.StudentId == studentId)
            .FirstOrDefaultAsync();
        if (studentStatistics is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetNotFoundMessage(Lang) });
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
        return Ok(new IsSuccessDto
        {
            IsSuccess = true,
            Message = Lang == Languages.En ? "Attendance added successfully" : "تمت إضافة الحضور بنجاح"
        });
    }

    [HttpPost("AddStudent")]
    [Authorize(Roles = nameof(Roles.SuperAdmin) + "," + nameof(Roles.Admin))]
    /// <summary>
    /// Adds a new student to the system. SuperAdmins can add to any major in their faculty; Admins can only add to their assigned major.
    /// </summary>
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
                return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
            }
            var superAdmin = await dbContext.SuperAdmins.FirstOrDefaultAsync(sa => sa.Id == superAdminId);
            if (superAdmin is null)
            {
                return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
            }
            var major = await dbContext.Majors.FirstOrDefaultAsync(m => m.Id == dto.MajorId);
            if (major is null || major.FacultyId != superAdmin.FacultyId)
            {
                return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
            }
        }
        else if (userRole == Roles.Admin)
        {
            // Admin: can add only to their assigned major
            if (adminId is null)
            {
                return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
            }
            var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
            if (admin is null || admin.MajorId != dto.MajorId)
            {
                return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
            }
        }
        else
        {
            return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
        }

        // Check for duplicate student number in the same major
        var exists = await dbContext.StudentCredentials.AnyAsync(sc => sc.StudentNumber == dto.StudentNumber && sc.MajorId == dto.MajorId);
        if (exists)
        {
            return BadRequest(new { message = Lang == Languages.En ? "A student with this number already exists in the major." : "يوجد طالب بهذا الرقم بالفعل في التخصص" });
        }

        // Get a default EnrollmentStatusId (first available)
        var defaultEnrollmentStatus = await dbContext.EnrollmentStatuses.FirstOrDefaultAsync();
        if (defaultEnrollmentStatus is null)
        {
            return BadRequest(new { message = Lang == Languages.En ? "No enrollment status found in the system." : "لم يتم العثور على حالة التسجيل في النظام" });
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
        return Ok(new { message = Lang == Languages.En ? "Student added successfully." : "تمت إضافة الطالب بنجاح", studentId = student.Id });
    }

    [HttpPost("UploadProfileImage")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> UploadProfileImage(IFormFile image)
    {
        try
        {
            var studentId = HttpContext.User.GetStudentId();
            if (studentId is null)
            {
                return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
            }

            var student = await dbContext.StudentCredentials.FirstOrDefaultAsync(sc => sc.Id == studentId);
            if (student is null)
            {
                return NotFound(new { message = BilingualErrorMessages.GetStudentNotFoundMessage(Lang) });
            }

            var imageUrl = await profileImageService.UploadProfileImageAsync(image, "student-profiles");
            student.Image = imageUrl;
            await dbContext.SaveChangesAsync();

            return Ok(new
            {
                message = Lang == Languages.En ? "Profile image uploaded successfully" : "تم رفع صورة الملف الشخصي بنجاح",
                imageUrl,
                fileName = image.FileName,
                fileSize = image.Length
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("EligibleStudentsForSubject")]
    /// <summary>
    /// Returns eligible students for a subject, enforcing access control for Admins and SuperAdmins.
    /// </summary>
    public async Task<ActionResult<DTOs.Info.EligibleStudentsCollectionDto>> GetEligibleStudentsForSubject([FromQuery] Guid subjectId)
    {
        var adminId = HttpContext.User.GetAdminId();
        var superAdminId = HttpContext.User.GetSuperAdminId();

        if (adminId is null && superAdminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        // Verify subject exists
        var subject = await dbContext.Subjects
            .Include(s => s.Major)
            .FirstOrDefaultAsync(s => s.Id == subjectId);
        if (subject is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetSubjectNotFoundMessage(Lang) });
        }

        // For admin, verify they can only access students from their major
        if (adminId is not null)
        {
            var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
            if (admin is null)
            {
                return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
            }

            if (admin.MajorId != subject.MajorId)
            {
                return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
            }
        }

        // For super admin, verify they can only access students from their faculty
        if (superAdminId is not null)
        {
            var superAdmin = await dbContext.SuperAdmins.FirstOrDefaultAsync(sa => sa.Id == superAdminId);
            if (superAdmin is null)
            {
                return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
            }

            if (subject.Major.FacultyId != superAdmin.FacultyId)
            {
                return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
            }
        }

        // Get eligible students (students in the same major and year as the subject)
        var eligibleStudents = await dbContext.StudentCredentials
            .Where(sc => sc.MajorId == subject.MajorId && sc.Year == subject.Year)
            .ToListAsync();

        return Ok(new EligibleStudentsCollectionDto
        {
            Students = eligibleStudents.Select(s => s.ToEligibleStudentDto(Lang)).ToList()
        });
    }
}
