using FluentValidation;

namespace UniSphere.Api.DTOs.Auth;

public class LoginStudentDtoValidator : AbstractValidator<LoginStudentDto>
{
    public LoginStudentDtoValidator()
    {
        RuleFor(x => x.StudentNumber).NotEmpty();
        RuleFor(x => x.MajorId).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
} 