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

namespace UniSphere.Api.Controllers;

[Authorize]
[ApiController]
[Produces("application/json")]

[Route("api/[controller]")]
public sealed class SubjectController(ApplicationDbContext dbContext) : BaseController
{
    [HttpGet("MySubjects")]
    public async Task<ActionResult<SubjectCollectionDto>> GetMySubjects()
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
            .Select(link => link.Subject)
            .Distinct() // Optional: if multiple links to the same subject exist
            .OrderBy(subject => subject.Year)
            .ThenBy(subject => subject.Semester)
            .Select(SubjectQueries.ProjectToDto(studentId.Value, Lang))
            .ToListAsync();

        return Ok(new SubjectCollectionDto { Subjects = subjects });
    }
    [HttpGet("GetMyMajorSubjects")]             
    public async Task<ActionResult<SubjectCollectionDto>> GetMyMajorSubjects( [Required] int year)
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            
            return Unauthorized();
        }

        var majorId = await dbContext.StudentCredentials.Where(sd => sd.Id == studentId
        ).Select(
            sd => sd.MajorId
        ).FirstOrDefaultAsync();
        var subjectCollectionDto = new SubjectCollectionDto
        {
            Subjects = await dbContext.Subjects
                .Where(subject => subject.MajorId == majorId && subject.Year == year)
                .Include(subject => subject.SubjectLecturers!)
                .ThenInclude(sl => sl.Professor!)
                .Include(subject => subject.SubjectStudentLinks!)
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
            
            return Unauthorized();
        }
        var subject = await dbContext.Subjects
            .Where(s => s.Id == id)
            .Include(s => s.Major)
            .Include(s => s.SubjectLecturers!)
                .ThenInclude(sl => sl.Professor!)
            .Include(s => s.SubjectStudentLinks!)
            .Select(SubjectQueries.ProjectToDto(studentId.Value,Lang))
            .FirstOrDefaultAsync();
        if (subject is null)
        {
            return NotFound();
        }

        return Ok(subject);
    }

    [HttpPost]
    public async Task<ActionResult<SubjectDto>> AddSubject(CreateSubjectDto createSubjectDto,
        IValidator<CreateSubjectDto> validator)
    {
        await validator.ValidateAndThrowAsync(createSubjectDto);
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            
            return Unauthorized();
        }

        if (!await dbContext.Majors.AnyAsync(mj => mj.Id == createSubjectDto.MajorId))
        {
            return Problem(
                detail: "The specified Major does not exist.",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        var subject = createSubjectDto.ToEntity();

        dbContext.Subjects.Add(subject);
        await dbContext.SaveChangesAsync();
        var subjectDto = subject.ToDto(studentId.Value,Lang);
        return CreatedAtAction(nameof(GetSubjectById), new { id = subject.Id }, subjectDto);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<SubjectDto>> UpdateSubject(Guid id, JsonPatchDocument<SubjectDto> pathDocument)
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            
            return Unauthorized();
        }
        var subject = await dbContext.Subjects.FindAsync(id);
        if (subject is null)
        {
            return NotFound();
        }

        var subjectDto = subject.ToDto(studentId.Value,Lang);
        pathDocument.ApplyTo(subjectDto, ModelState);
        if (!TryValidateModel(subjectDto))
        {
            return ValidationProblem(ModelState);
        }

        if (!await dbContext.Majors.AnyAsync(mj => mj.Id == subjectDto.MajorId))
        {
            return Problem(
                detail: "The specified Major does not exist.",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        subject = subject.UpdateFromDto(subjectDto);
        await dbContext.SaveChangesAsync();
        return Ok(subject.ToDto(studentId.Value,Lang));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteSubject(Guid id)
    {
        Subject? subject = await dbContext.Subjects.FindAsync(id);
        if (subject is null)
        {
            return NotFound();
        }

        dbContext.Subjects.Remove(subject);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
