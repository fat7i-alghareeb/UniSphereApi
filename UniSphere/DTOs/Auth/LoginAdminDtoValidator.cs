using FluentValidation;

namespace UniSphere.Api.DTOs.Auth;

public class LoginAdminDtoValidator : AbstractValidator<LoginAdminDto>
{
    public LoginAdminDtoValidator()
    {
        RuleFor(x => x.Gmail).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
} 