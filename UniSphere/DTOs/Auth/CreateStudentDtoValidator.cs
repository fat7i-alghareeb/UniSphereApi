using FluentValidation;

namespace UniSphere.Api.DTOs.Auth;

public class CreateStudentDtoValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentDtoValidator()
    {
        RuleFor(x => x.StudentNumber).NotEmpty();
        RuleFor(x => x.MajorId).NotEmpty();
        RuleFor(x => x.FirstNameEn).NotEmpty();
        RuleFor(x => x.FirstNameAr).NotEmpty();
        RuleFor(x => x.LastNameEn).NotEmpty();
        RuleFor(x => x.LastNameAr).NotEmpty();
        RuleFor(x => x.Year).NotEmpty();
    }
} 
