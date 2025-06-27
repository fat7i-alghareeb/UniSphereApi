using FluentValidation;

namespace UniSphere.Api.DTOs.Subjects;

public sealed record CreateSubjectDto
{
    public required Guid MajorId { get; init; }
    public required string NameEn { get; init; }
    public required string NameAr { get; init; }
    public required string DescriptionEn { get; init; }
    public required string DescriptionAr { get; init; }
    public required int Year { get; init; }
    public required int Semester { get; init; }
    public required int MidtermGrade { get; init; } = 30;
    public required int FinalGrade { get; init; } = 70;
    public required bool IsLabRequired { get; init; }
    public required bool IsMultipleChoice { get; init; }
    public required bool IsOpenBook { get; init; }
    public Guid? LabId { get; init; }
    public string? Image { get; init; }
}

public class CreateSubjectDtoValidator : AbstractValidator<CreateSubjectDto>
{
    public CreateSubjectDtoValidator()
    {
        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("English name is required.")
            .MaximumLength(100).WithMessage("English name cannot exceed 100 characters.");
            
        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Arabic name is required.")
            .MaximumLength(100).WithMessage("Arabic name cannot exceed 100 characters.");
            
        RuleFor(x => x.DescriptionEn)
            .NotEmpty().WithMessage("English description is required.")
            .MaximumLength(500).WithMessage("English description cannot exceed 500 characters.");
            
        RuleFor(x => x.DescriptionAr)
            .NotEmpty().WithMessage("Arabic description is required.")
            .MaximumLength(500).WithMessage("Arabic description cannot exceed 500 characters.");
            
        RuleFor(x => x.MajorId) 
            .NotEmpty().WithMessage("MajorId is required.");
            
        RuleFor(x => x.Year)
            .GreaterThan(0).WithMessage("Year must be greater than 0.")
            .LessThanOrEqualTo(10).WithMessage("Year cannot exceed 10.");
            
        RuleFor(x => x.Semester)
            .GreaterThan(0).WithMessage("Semester must be greater than 0.")
            .LessThanOrEqualTo(4).WithMessage("Semester cannot exceed 4.");
            
        RuleFor(x => x.MidtermGrade)
            .GreaterThanOrEqualTo(0).WithMessage("Midterm grade cannot be negative.")
            .LessThanOrEqualTo(100).WithMessage("Midterm grade cannot exceed 100.");
            
        RuleFor(x => x.FinalGrade)
            .GreaterThanOrEqualTo(0).WithMessage("Final grade cannot be negative.")
            .LessThanOrEqualTo(100).WithMessage("Final grade cannot exceed 100.");
            
        RuleFor(x => x.MidtermGrade + x.FinalGrade)
            .Equal(100).WithMessage("Midterm and final grades must sum to 100.");
    }
}
