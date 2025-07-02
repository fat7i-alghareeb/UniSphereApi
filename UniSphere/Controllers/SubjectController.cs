using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Subjects;
using UniSphere.Api.Entities;
using UniSphere.Api.Extensions;
using UniSphere.Api.Services;
using UniSphere.Api.Helpers;

namespace UniSphere.Api.Controllers;

[Authorize]
[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public sealed class SubjectController(ApplicationDbContext dbContext, IStorageService storageService) : BaseController
{
    [HttpGet("Student/MySubjects")]
    public async Task<ActionResult<SubjectCollectionDto>> GetMySubjects()
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
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
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        // Get all relevant subject IDs (executes immediately due to ToListAsync)
        var subjectIds = await dbContext.Subjects
            .Where(st => st.MajorId == studentInfo.MajorId &&
                            st.Year <= studentInfo.Year &&
                            st.Semester <= 2)
            .Select(s => s.Id)
            .ToListAsync();

        // Process enrollments
        foreach (var subjectId in subjectIds)
        {
            if (!await dbContext.SubjectStudentLinks
                    .AnyAsync(s => s.SubjectId == subjectId))
            {
                await dbContext.SubjectStudentLinks.AddAsync(new SubjectStudentLink
                {
                    SubjectId = subjectId,
                    StudentId = studentId.Value,
                    AttemptNumber = 0,
                    IsCurrentlyEnrolled = true,
                    IsPassed = false,
                });
            }
        }

        await dbContext.SaveChangesAsync();

        // Now get the enrolled subjects
        var subjects = await dbContext.SubjectStudentLinks
            .Where(link => link.StudentId == studentId)
            .Include(link => link.Subject)
            .ThenInclude(subject => subject.Major)
            .Include(link => link.Subject)
            .ThenInclude(subject => subject.SubjectStudentLinks!)
            .Include(link => link.Subject)
            .ThenInclude(subject => subject.SubjectLecturers!)
            .ThenInclude(sl => sl.Professor!)
            .Include(link => link.Subject)
            .ThenInclude(subject => subject.Materials!)
            .Select(link => link.Subject)
            .Distinct() // Optional: if multiple links to the same subject exist
            .OrderBy(subject => subject.Year)
            .ThenBy(subject => subject.Semester)
            .Select(SubjectQueries.ProjectToDto(studentId.Value, Lang))
            .ToListAsync();

        return Ok(new SubjectCollectionDto { Subjects = subjects });
    }

    [HttpGet("Student/GetMyMajorSubjects")]
    public async Task<ActionResult<SubjectCollectionDto>> GetMyMajorSubjects([Required] int year)
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var majorId = await dbContext.StudentCredentials.Where(sd => sd.Id == studentId
        ).Select(sd => sd.MajorId
        ).FirstOrDefaultAsync();
        var subjectCollectionDto = new SubjectCollectionDto
        {
            Subjects = await dbContext.Subjects
                .Where(subject => subject.MajorId == majorId && subject.Year == year)
                .Include(subject => subject.SubjectLecturers!)
                .ThenInclude(sl => sl.Professor!)
                .Include(subject => subject.SubjectStudentLinks!)
                .Include(subject => subject.Materials!)
                .OrderBy(subject => subject.Semester) // ✅ Move before projection
                .Select(SubjectQueries.ProjectToDto(studentId.Value, Lang))
                .ToListAsync()
        };

        return Ok(subjectCollectionDto);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SubjectDto>> GetSubjectById(Guid id)
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var subject = await dbContext.Subjects
            .Where(s => s.Id == id)
            .Include(s => s.Major)
            .Include(s => s.SubjectLecturers!)
            .ThenInclude(sl => sl.Professor!)
            .Include(s => s.SubjectStudentLinks!)
            .Include(s => s.Materials!)
            .Select(SubjectQueries.ProjectToDto(studentId.Value, Lang))
            .FirstOrDefaultAsync();
        if (subject is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetSubjectNotFoundMessage(Lang) });
        }

        return Ok(subject);
    }

    // New endpoints for SuperAdmins and Professors

    [HttpGet("SuperAdmin/Subjects")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult<SuperAdminSubjectsResponseDto>> GetSuperAdminSubjects(
        [Required] int year, 
        [Required] Guid majorId)
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

        // Get subjects filtered by faculty, year, and major
        var subjects = await dbContext.Subjects
            .Where(s => s.MajorId == majorId && 
                       s.Year == year &&
                       s.Major.FacultyId == facultyId)
            .Include(s => s.Major)
            .Include(s => s.Materials!)
            .OrderBy(s => s.Semester)
            .Select(SubjectQueries.ProjectToUnifiedDto(Lang))
            .ToListAsync();

        var major = await dbContext.Majors
            .Where(m => m.Id == majorId)
            .FirstOrDefaultAsync();

        if (major is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetMajorNotFoundMessage(Lang) });
        }

        var response = new SuperAdminSubjectsResponseDto
        {
            Majors = new List<MajorSubjectsDto>
            {
                new MajorSubjectsDto
                {
                    MajorName = major.Name.GetTranslatedString(Lang),
                    Subjects = subjects
                }
            }
        };

        return Ok(response);
    }

    [HttpGet("Professor/Subjects")]
    [Authorize(Roles = Roles.Professor)]
    public async Task<ActionResult<ProfessorSubjectsResponseDto>> GetProfessorSubjects()
    {
        var professorId = HttpContext.User.GetProfessorId();
        if (professorId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        // Get all subject IDs associated with the professor
        var subjectIds = await dbContext.SubjectProfessorLinks
            .Where(spl => spl.ProfessorId == professorId)
            .Select(spl => spl.SubjectId)
            .ToListAsync();

        if (!subjectIds.Any())
        {
            return Ok(new ProfessorSubjectsResponseDto
            {
                UniversityName = "",
                Faculties = new List<FacultySubjectsDto>()
            });
        }

        // Get all subjects with their related data
        var professorSubjects = await dbContext.Subjects
            .Where(s => subjectIds.Contains(s.Id))
            .Include(s => s.Major)
            .ThenInclude(m => m.Faculty)
            .ThenInclude(f => f.University)
            .Include(s => s.Materials!)
            .ToListAsync();

        if (!professorSubjects.Any())
        {
            return Ok(new ProfessorSubjectsResponseDto
            {
                UniversityName = "",
                Faculties = new List<FacultySubjectsDto>()
            });
        }

        // Get university name from the first faculty (all faculties should belong to the same university)
        var universityName = professorSubjects[0].Major.Faculty.University.Name.GetTranslatedString(Lang);

        var faculties = professorSubjects
            .GroupBy(s => s.Major.Faculty)
            .Select(facultyGroup => new FacultySubjectsDto
            {
                FacultyName = facultyGroup.Key.Name.GetTranslatedString(Lang),
                Majors = facultyGroup
                    .GroupBy(s => s.Major)
                    .Select(majorGroup => new MajorSubjectsDto
                    {
                        MajorName = majorGroup.Key.Name.GetTranslatedString(Lang),
                        Subjects = majorGroup
                            .OrderBy(s => s.Year)
                            .ThenBy(s => s.Semester)
                            .Select(s => s.ToUnifiedDto(Lang))
                            .ToList()
                    })
                    .ToList()
            })
            .ToList();

        var response = new ProfessorSubjectsResponseDto
        {
            UniversityName = universityName,
            Faculties = faculties
        };

        return Ok(response);
    }

    [HttpGet("SuperAdmin/{id:guid}")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult<UnifiedSubjectDto>> GetSuperAdminSubjectById(Guid id)
    {
        var superAdminId = HttpContext.User.GetSuperAdminId();
        if (superAdminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var subject = await dbContext.Subjects
            .Where(s => s.Id == id)
            .Include(s => s.Materials!)
            .Select(SubjectQueries.ProjectToUnifiedDto(Lang))
            .FirstOrDefaultAsync();

        if (subject is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetSubjectNotFoundMessage(Lang) });
        }

        return Ok(subject);
    }

    [HttpGet("Professor/{id:guid}")]
    [Authorize(Roles = Roles.Professor)]
    public async Task<ActionResult<UnifiedSubjectDto>> GetProfessorSubjectById(Guid id)
    {
        var professorId = HttpContext.User.GetProfessorId();
        if (professorId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        // Verify the professor has access to this subject
        var hasAccess = await dbContext.SubjectProfessorLinks
            .AnyAsync(spl => spl.ProfessorId == professorId && spl.SubjectId == id);

        if (!hasAccess)
        {
            return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
        }

        var subject = await dbContext.Subjects
            .Where(s => s.Id == id)
            .Include(s => s.Materials!)
            .Select(SubjectQueries.ProjectToUnifiedDto(Lang))
            .FirstOrDefaultAsync();

        if (subject is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetSubjectNotFoundMessage(Lang) });
        }

        return Ok(subject);
    }

    [HttpPost]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult<UnifiedSubjectDto>> AddSubject(CreateSubjectDto createSubjectDto,
        IValidator<CreateSubjectDto> validator)
    {
        await validator.ValidateAndThrowAsync(createSubjectDto);

        if (!await dbContext.Majors.AnyAsync(mj => mj.Id == createSubjectDto.MajorId))
        {
            return BadRequest(new { message = BilingualErrorMessages.GetMajorNotFoundMessage(Lang) });
        }

        if (createSubjectDto.LabId.HasValue && !await dbContext.Labs.AnyAsync(l => l.Id == createSubjectDto.LabId))
        {
            return BadRequest(new { message = BilingualErrorMessages.GetLabNotFoundMessage(Lang) });
        }

        var subject = createSubjectDto.ToEntity();

        dbContext.Subjects.Add(subject);
        await dbContext.SaveChangesAsync();
        
        var subjectDto = subject.ToUnifiedDto(Lang);
        return CreatedAtAction(nameof(GetSuperAdminSubjectById), new { id = subject.Id }, subjectDto);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult<UnifiedSubjectDto>> UpdateSubject(Guid id, JsonPatchDocument<UnifiedSubjectDto> patchDocument)
    {
        var subject = await dbContext.Subjects.FindAsync(id);
        if (subject is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetSubjectNotFoundMessage(Lang) });
        }

        var subjectDto = subject.ToUnifiedDto(Lang);
        patchDocument.ApplyTo(subjectDto, ModelState);
        if (!TryValidateModel(subjectDto))
        {
            return ValidationProblem(ModelState);
        }

        // Update the subject with the patched values
        subject.Name = new MultilingualText { En = subjectDto.Name, Ar = subjectDto.Name };
        subject.Description = new MultilingualText { En = subjectDto.Description, Ar = subjectDto.Description };
        subject.MajorId = subjectDto.MajorId;
        subject.LabId = subjectDto.LabId;
        subject.Year = subjectDto.Year;
        subject.Semester = subjectDto.Semester;
        subject.MidtermGrade = subjectDto.MidtermGrade;
        subject.FinalGrade = subjectDto.FinalGrade;
        subject.IsLabRequired = subjectDto.IsLabRequired;
        subject.IsMultipleChoice = subjectDto.IsMultipleChoice;
        subject.IsOpenBook = subjectDto.IsOpenBook;
        subject.Image = subjectDto.Image;

        await dbContext.SaveChangesAsync();
        return Ok(subject.ToUnifiedDto(Lang));
    }

    [HttpPost("{id:guid}/materials")]
    [Authorize(Roles = Roles.Professor)]
    public async Task<ActionResult<UnifiedSubjectDto>> UploadMaterial(Guid id, [FromForm] UploadMaterialDto uploadDto)
    {
        var professorId = HttpContext.User.GetProfessorId();
        if (professorId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        // Verify the professor has access to this subject
        var hasAccess = await dbContext.SubjectProfessorLinks
            .AnyAsync(spl => spl.ProfessorId == professorId && spl.SubjectId == id);

        if (!hasAccess)
        {
            return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
        }

        // Verify the subject exists
        var subjectExists = await dbContext.Subjects
            .Include(s => s.Materials)
            .AnyAsync(s => s.Id == id);
        if (!subjectExists)
        {
            return NotFound(new { message = BilingualErrorMessages.GetSubjectNotFoundMessage(Lang) });
        }

        if (!uploadDto.IsValid())
        {
            return BadRequest(new { message = BilingualErrorMessages.GetMaterialUploadRequiredMessage(Lang) });
        }

        // Validate link format if it's a link upload
        if (uploadDto.IsLinkUpload && !LocalStorageService.IsValidUrl(uploadDto.Link!))
        {
            return BadRequest(new { message = BilingualErrorMessages.GetInvalidLinkFormatMessage(Lang) });
        }

        try
        {
            string materialUrl;
            string materialType;

            if (uploadDto.IsFileUpload)
            {
                // Handle file upload
                materialUrl = await storageService.SaveFileAsync(uploadDto.File!);
                materialType = uploadDto.CustomType ?? LocalStorageService.GetMaterialTypeFromUrl(uploadDto.File!.FileName);
            }
            else
            {
                // Handle link upload
                materialUrl = uploadDto.Link!;
                materialType = uploadDto.CustomType ?? LocalStorageService.GetMaterialTypeFromUrl(uploadDto.Link!);
            }

            // Create the material record
            var material = new Material
            {
                Id = Guid.NewGuid(),
                SubjectId = id,
                Url = materialUrl,
                Type = materialType,
                CreatedAt = DateTime.UtcNow
            };

            dbContext.Materials.Add(material);
            await dbContext.SaveChangesAsync();

            // Reload the subject with materials
            var updatedSubject = await dbContext.Subjects
                .Where(s => s.Id == id)
                .Include(s => s.Materials)
                .Select(SubjectQueries.ProjectToUnifiedDto(Lang))
                .FirstOrDefaultAsync();

            if (updatedSubject is null)
            {
                return NotFound(new { message = BilingualErrorMessages.GetSubjectNotFoundMessage(Lang) });
            }

            return Ok(updatedSubject);
        }
        catch (Exception ex)
        {
            return Problem(
                detail: Lang == Languages.En ? $"Error uploading material: {ex.Message}" : $"حدث خطأ أثناء رفع المادة: {ex.Message}",
                statusCode: StatusCodes.Status500InternalServerError
            );
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.SuperAdmin)]
    public async Task<ActionResult> DeleteSubject(Guid id)
    {
        Subject? subject = await dbContext.Subjects.FindAsync(id);
        if (subject is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetSubjectNotFoundMessage(Lang) });
        }

        dbContext.Subjects.Remove(subject);
        await dbContext.SaveChangesAsync();
        return Ok(new { message = Lang == Languages.En ? "Subject deleted successfully" : "تم حذف المادة بنجاح" });
    }
}
