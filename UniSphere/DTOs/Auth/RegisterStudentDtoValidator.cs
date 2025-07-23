using FluentValidation;

namespace UniSphere.Api.DTOs.Auth;

public class RegisterStudentDtoValidator : AbstractValidator<RegisterStudentDto>
{
    public RegisterStudentDtoValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(5);
        RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x => x.Password).WithMessage("Passwords must match");
    }
} 