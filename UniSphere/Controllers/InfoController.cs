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
using UniSphere.Api.DTOs.Auth;

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
    public async Task<ActionResult<SubjectNameIdCollectionDto>> GetAdminMajorSubjects([Required] int year)
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

        return Ok(new SubjectNameIdCollectionDto { Subjects = subjects });
    }

    [HttpGet("Subject/{subjectId:guid}/EligibleStudents")]
    public async Task<ActionResult<EligibleStudentsCollectionDto>> GetEligibleStudentsForSubject(Guid subjectId)
    {
        var students = await dbContext.SubjectStudentLinks
            .Where(link => link.SubjectId == subjectId && link.IsCurrentlyEnrolled && !link.IsPassed)
            .Include(link => link.StudentCredential)
            .Select(link => link.StudentCredential)
            .Distinct()
            .ToListAsync();

        var result = new EligibleStudentsCollectionDto
        {
            Students = students.Select(s => s.ToEligibleStudentDto(Lang)).ToList()
        };
        return Ok(result);
    }

    // Task 1: Get Unassigned Subjects (SuperAdmin only)
    [HttpGet("SuperAdmin/GetUnassignedSubjects")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<SubjectNameIdCollectionDto>> GetUnassignedSubjects([FromQuery] Guid majorId, [FromQuery] int majorYear)
    {
        // Only subjects in the given major and year, with no assigned professor
        var subjects = await dbContext.Subjects
            .Where(s => s.MajorId == majorId && s.Year == majorYear)
            .Include(s => s.SubjectLecturers)
            .Where(s => s.SubjectLecturers == null || !s.SubjectLecturers.Any())
            .Select(s => s.ToSubjectNameIdDto(Lang))
            .ToListAsync();
        return Ok(new SubjectNameIdCollectionDto { Subjects = subjects });
    }

    // Task 2: Get Professors by Faculty (SuperAdmin only)
    [HttpGet("SuperAdmin/GetProfessorsByFaculty")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<SimpleProfessorCollectionDto>> GetProfessorsByFaculty()
    {
        var superAdminId = HttpContext.User.GetSuperAdminId();
        if (superAdminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }
        var facultyId = await dbContext.SuperAdmins
            .Where(sa => sa.Id == superAdminId)
            .Select(sa => sa.FacultyId)
            .FirstOrDefaultAsync();
        if (facultyId == Guid.Empty)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }
        var professors = await dbContext.ProfessorFacultyLinks
            .Where(pfl => pfl.FacultyId == facultyId)
            .Include(pfl => pfl.Professor)
            .Select(pfl => pfl.Professor)
            .Distinct()
            .Select(p => AuthMappings.ToSimpleProfessorDto(p))
            .ToListAsync();
        return Ok(new SimpleProfessorCollectionDto { Professors = professors });
    }

    // Task 3: Get Unregistered Students by Major (Admin only)
    [HttpGet("Admin/GetUnregisteredStudentsByMajor")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<AdminIdNameDto>> GetUnregisteredStudentsByMajor([FromQuery] string studentNumber)
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
        var student = await dbContext.StudentCredentials
            .Where(sc => sc.MajorId == majorId && sc.IdentityId == null && sc.StudentNumber == studentNumber)
            .Include(sc => sc.Major)
            .Select(sc => new AdminIdNameDto
            {
                Id = sc.Id,
                Name = sc.FirstName.GetTranslatedString(Lang) + " " + sc.LastName.GetTranslatedString(Lang)
            })
            .SingleOrDefaultAsync();
        if (student == null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetNotFoundMessage(Lang) });
        }
        return Ok(student);
    }

    // Task 4: Get Unregistered Admins by Faculty (SuperAdmin only)
    [HttpGet("SuperAdmin/GetUnregisteredAdminsByFaculty")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<AdminIdNameCollectionDto>> GetUnregisteredAdminsByFaculty()
    {
        var superAdminId = HttpContext.User.GetSuperAdminId();
        if (superAdminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }
        var facultyId = await dbContext.SuperAdmins
            .Where(sa => sa.Id == superAdminId)
            .Select(sa => sa.FacultyId)
            .FirstOrDefaultAsync();
        if (facultyId == Guid.Empty)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }
        var admins = await dbContext.Admins
            .Include(a => a.Major)
            .Where(a => a.IdentityId == null && a.Major.FacultyId == facultyId)
            .Select(a => new AdminIdNameDto
            {
                Id = a.Id,
                Name = a.FirstName.GetTranslatedString(Lang) + " " + a.LastName.GetTranslatedString(Lang)
            })
            .ToListAsync();
        return Ok(new AdminIdNameCollectionDto { Admins = admins });
    }

    // Returns all professors (Id and Name only) for SuperAdmin or Admin
    [HttpGet("GetProfessorsIdName")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<ActionResult<ProfessorIdNameCollectionDto>> GetProfessorsIdName()
    {
        var professors = await dbContext.Professors
            .Select(p => new ProfessorIdNameDto
            {
                Id = p.Id,
                Name = p.FirstName.GetTranslatedString(Lang) + " " + p.LastName.GetTranslatedString(Lang)
            })
            .ToListAsync();
        return Ok(new ProfessorIdNameCollectionDto { Professors = professors });
    }

    // Returns all admins (Id and Name only) for SuperAdmin only
    [HttpGet("GetAdminsIdName")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<AdminIdNameCollectionDto>> GetAdminsIdName()
    {
        var admins = await dbContext.Admins
            .Select(a => new AdminIdNameDto
            {
                Id = a.Id,
                Name = a.FirstName.GetTranslatedString(Lang) + " " + a.LastName.GetTranslatedString(Lang)
            })
            .ToListAsync();
        return Ok(new AdminIdNameCollectionDto { Admins = admins });
    }

    // Returns all unregistered professors (Id and Name only) for SuperAdmin's faculty
    [HttpGet("SuperAdmin/GetUnregisteredProfessors")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<ActionResult<ProfessorIdNameCollectionDto>> GetUnregisteredProfessors()
    {
        var superAdminId = HttpContext.User.GetSuperAdminId();
        if (superAdminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }
        var facultyId = await dbContext.SuperAdmins
            .Where(sa => sa.Id == superAdminId)
            .Select(sa => sa.FacultyId)
            .FirstOrDefaultAsync();
        if (facultyId == Guid.Empty)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }
        var professors = await dbContext.ProfessorFacultyLinks
            .Include(pfl => pfl.Professor)
            .Where(pfl => pfl.FacultyId == facultyId && pfl.Professor.IdentityId == null)
            .Select(pfl => pfl.Professor)
            .Distinct()
            .Select(p => new ProfessorIdNameDto
            {
                Id = p.Id,
                Name = p.FirstName.GetTranslatedString(Lang) + " " + p.LastName.GetTranslatedString(Lang)
            })
            .ToListAsync();
        return Ok(new ProfessorIdNameCollectionDto { Professors = professors });
    }
}

