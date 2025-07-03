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
    public required double PassGrade { get; init; }
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
            .NotEmpty().WithMessage("English name is required. | الاسم بالإنجليزية مطلوب.")
            .MaximumLength(100).WithMessage("English name cannot exceed 100 characters. | لا يمكن أن يتجاوز الاسم بالإنجليزية 100 حرف.");
            
        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Arabic name is required. | الاسم بالعربية مطلوب.")
            .MaximumLength(100).WithMessage("Arabic name cannot exceed 100 characters. | لا يمكن أن يتجاوز الاسم بالعربية 100 حرف.");
            
        RuleFor(x => x.DescriptionEn)
            .NotEmpty().WithMessage("English description is required. | الوصف بالإنجليزية مطلوب.")
            .MaximumLength(500).WithMessage("English description cannot exceed 500 characters. | لا يمكن أن يتجاوز الوصف بالإنجليزية 500 حرف.");
            
        RuleFor(x => x.DescriptionAr)
            .NotEmpty().WithMessage("Arabic description is required. | الوصف بالعربية مطلوب.")
            .MaximumLength(500).WithMessage("Arabic description cannot exceed 500 characters. | لا يمكن أن يتجاوز الوصف بالعربية 500 حرف.");
            
        RuleFor(x => x.MajorId) 
            .NotEmpty().WithMessage("MajorId is required. | معرف التخصص مطلوب.");
            
        RuleFor(x => x.Year)
            .GreaterThan(0).WithMessage("Year must be greater than 0. | يجب أن تكون السنة أكبر من 0.")
            .LessThanOrEqualTo(10).WithMessage("Year cannot exceed 10. | لا يمكن أن تتجاوز السنة 10.");
            
        RuleFor(x => x.Semester)
            .GreaterThan(0).WithMessage("Semester must be greater than 0. | يجب أن يكون الفصل الدراسي أكبر من 0.")
            .LessThanOrEqualTo(4).WithMessage("Semester cannot exceed 4. | لا يمكن أن يتجاوز الفصل الدراسي 4.");
            
        RuleFor(x => x.MidtermGrade)
            .GreaterThanOrEqualTo(0).WithMessage("Midterm grade cannot be negative. | لا يمكن أن تكون درجة منتصف الفصل سالبة.")
            .LessThanOrEqualTo(100).WithMessage("Midterm grade cannot exceed 100. | لا يمكن أن تتجاوز درجة منتصف الفصل 100.");
            
        RuleFor(x => x.FinalGrade)
            .GreaterThanOrEqualTo(0).WithMessage("Final grade cannot be negative. | لا يمكن أن تكون درجة نهاية الفصل سالبة.")
            .LessThanOrEqualTo(100).WithMessage("Final grade cannot exceed 100. | لا يمكن أن تتجاوز درجة نهاية الفصل 100.");
            
        RuleFor(x => x.MidtermGrade + x.FinalGrade)
            .Equal(100).WithMessage("Midterm and final grades must sum to 100. | يجب أن يكون مجموع درجات منتصف الفصل ونهاية الفصل 100.");

        RuleFor(x => x.PassGrade)
            .NotNull().WithMessage("Pass grade is required. | درجة النجاح مطلوبة.")
            .GreaterThanOrEqualTo(0).WithMessage("Pass grade cannot be negative. | لا يمكن أن تكون درجة النجاح سالبة.")
            .LessThanOrEqualTo(100).WithMessage("Pass grade cannot exceed 100. | لا يمكن أن تتجاوز درجة النجاح 100.");
    }
}
