using FluentValidation;

namespace UniSphere.Api.DTOs.Auth;

public class LoginSystemControllerDtoValidator : AbstractValidator<LoginSystemControllerDto>
{
    public LoginSystemControllerDtoValidator()
    {
        RuleFor(x => x.Gmail).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
} 