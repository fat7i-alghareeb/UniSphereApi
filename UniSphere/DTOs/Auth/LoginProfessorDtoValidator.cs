using FluentValidation;

namespace UniSphere.Api.DTOs.Auth;

public class LoginProfessorDtoValidator : AbstractValidator<LoginProfessorDto>
{
    public LoginProfessorDtoValidator()
    {
        RuleFor(x => x.Gmail).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
} 