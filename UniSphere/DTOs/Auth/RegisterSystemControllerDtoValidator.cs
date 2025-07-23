using FluentValidation;

namespace UniSphere.Api.DTOs.Auth;

public class RegisterSystemControllerDtoValidator : AbstractValidator<RegisterSystemControllerDto>
{
    public RegisterSystemControllerDtoValidator()
    {
        RuleFor(x => x.UserName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(5);
        RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x => x.Password).WithMessage("Passwords must match");
        RuleFor(x => x.SystemControllerId).NotEmpty();
    }
} 