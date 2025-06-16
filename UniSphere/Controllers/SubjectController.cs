using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Subjects;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Controllers;

[Authorize(Roles = Roles.Admin)]
[ApiController]
[Route("api/[controller]")]

public sealed class SubjectController(ApplicationDbContext dbContext) : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<SubjectCollectionDto>> GetSubjects()
    {
        List<SubjectDto> subjects = await dbContext.Subjects.Select(SubjectQueries.ProjectToDto()).ToListAsync();
        var subjectCollectionDto = new SubjectCollectionDto { Subjects = subjects };
        return Ok(subjectCollectionDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubjectDto>> GetSubjectById(Guid id)
    {
        SubjectDto? subject = await dbContext.Subjects
            .Where(s => s.Id == id)
            .Select(SubjectQueries.ProjectToDto())
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


        if (!await dbContext.Majors.AnyAsync(mj => mj.Id == createSubjectDto.MajorId))
        {

            return Problem(
                    detail: "The specified Major does not exist.",
                    statusCode: StatusCodes.Status404NotFound
            );
        }
        Subject subject = createSubjectDto.ToEntity();

        dbContext.Subjects.Add(subject);
        await dbContext.SaveChangesAsync();
        SubjectDto subjectDto = subject.ToDto();
        return CreatedAtAction(nameof(GetSubjectById), new { id = subject.Id }, subjectDto);
    }

    [HttpPatch("{id}")]
    public async Task<ActionResult<SubjectDto>> UpdateSubject(Guid id, JsonPatchDocument<SubjectDto> pathDocument)
    {
        Subject? subject = await dbContext.Subjects.FindAsync(id);
        if (subject is null)
        {
            return NotFound();
        }
        SubjectDto subjectDto = subject.ToDto();
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
        return Ok(subject.ToDto());
    }

    [HttpDelete("{id}")]
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
