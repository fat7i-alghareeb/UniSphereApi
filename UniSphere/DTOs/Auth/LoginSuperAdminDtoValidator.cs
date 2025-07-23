using FluentValidation;

namespace UniSphere.Api.DTOs.Auth;

public class LoginSuperAdminDtoValidator : AbstractValidator<LoginSuperAdminDto>
{
    public LoginSuperAdminDtoValidator()
    {
        RuleFor(x => x.Gmail).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
} 