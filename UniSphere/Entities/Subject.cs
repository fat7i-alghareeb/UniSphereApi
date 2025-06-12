using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;

namespace UniSphere.Api.Entities;

public class Subject
{
    public Guid Id { get; set; }
    public MultilingualText Name { get; set; } = new();
    public Guid MajorId { get; set; }
    public Guid? LabId { get; set; }
    public MultilingualText? Description { get; set; }
    public int Year { get; set; }
    public int Semester { get; set; }
    public int MidtermGrade { get; set; }
    public int FinalGrade { get; set; }
    public bool IsLabRequired { get; set; }
    public bool IsMultipleChoice { get; set; }
    public bool IsOpenBook { get; set; }
    public string? Image { get; set; }
    
    public Major Major { get; set; } = null!;
    public Lab? Lab { get; set; }
    public List<SubjectProfessorLink>? SubjectLecturers { get; set; }
    public List<SubjectStudentLink>? SubjectStudentLinks { get; set; }
}

/* Commented out DTOs

public class CreateSubjectDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public Subject ToEntity()
    {
        return new Subject
        {
            Name = Name,
            Description = Description
        };
    }
}

public class CreateSubjectDtoValidator : AbstractValidator<CreateSubjectDto>
{
    public CreateSubjectDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}
*/
