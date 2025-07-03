using FluentValidation;

namespace UniSphere.Api.DTOs.Grades;

public sealed record AssignGradesDto
{
    public Guid? SubjectId { get; init; }
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
            .NotNull().WithMessage("SubjectId is required. | معرف المادة مطلوب.");
        RuleFor(x => x.StudentGrades)
            .NotNull().WithMessage("StudentGrades list cannot be null. | قائمة درجات الطلاب لا يمكن أن تكون فارغة.")
            .Must(list => list.Count > 0).WithMessage("StudentGrades list cannot be empty. | قائمة درجات الطلاب لا يمكن أن تكون فارغة.");
        RuleForEach(x => x.StudentGrades).SetValidator(new StudentGradeDtoValidator());
    }
}

public class StudentGradeDtoValidator : AbstractValidator<StudentGradeDto>
{
    public StudentGradeDtoValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty().WithMessage("StudentId is required. | معرف الطالب مطلوب.");
        RuleFor(x => x.MidTermGrade).GreaterThanOrEqualTo(0).When(x => x.MidTermGrade.HasValue)
            .WithMessage("MidTermGrade must be greater than or equal to 0. | يجب أن تكون درجة منتصف الفصل أكبر من أو تساوي 0.");
        RuleFor(x => x.FinalGrade).GreaterThanOrEqualTo(0).When(x => x.FinalGrade.HasValue)
            .WithMessage("FinalGrade must be greater than or equal to 0. | يجب أن تكون درجة نهاية الفصل أكبر من أو تساوي 0.");
    }
} 