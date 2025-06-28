using FluentValidation;

namespace UniSphere.Api.DTOs.Grades;

public sealed record AssignGradesDto
{
    public Guid? SubjectId { get; init; }
    public double? PassGrade { get; init; }
    public List<StudentGradeDto> StudentGrades { get; init; } = new();
}

public sealed record StudentGradeDto
{
    public Guid StudentId { get; init; }
    public double? MidTermGrade { get; init; }
    public double? FinalGrade { get; init; }
}

public class AssignGradesDtoValidator : AbstractValidator<AssignGradesDto>
{
    public AssignGradesDtoValidator()
    {
        RuleFor(x => x.SubjectId)
            .NotNull().WithMessage("SubjectId is required.");
        RuleFor(x => x.PassGrade)
            .NotNull().WithMessage("PassGrade is required.")
            .GreaterThanOrEqualTo(0).WithMessage("PassGrade must be greater than or equal to 0.");
        RuleFor(x => x.StudentGrades)
            .NotNull().WithMessage("StudentGrades list cannot be null.")
            .Must(list => list.Count > 0).WithMessage("StudentGrades list cannot be empty.");
        RuleForEach(x => x.StudentGrades).SetValidator(new StudentGradeDtoValidator());
    }
}

public class StudentGradeDtoValidator : AbstractValidator<StudentGradeDto>
{
    public StudentGradeDtoValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.MidTermGrade).GreaterThanOrEqualTo(0).When(x => x.MidTermGrade.HasValue);
        RuleFor(x => x.FinalGrade).GreaterThanOrEqualTo(0).When(x => x.FinalGrade.HasValue);
    }
} 