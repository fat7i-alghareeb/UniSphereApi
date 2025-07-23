using FluentValidation;

namespace UniSphere.Api.DTOs.Auth;

public class AssignOneTimeCodeRequestDtoValidator : AbstractValidator<AssignOneTimeCodeRequestDto>
{
    public AssignOneTimeCodeRequestDtoValidator()
    {
        RuleFor(x => x.TargetRole).NotNull();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.OneTimeCode).GreaterThan(0).When(x => x.OneTimeCode.HasValue);
        RuleFor(x => x.ExpirationInMinutes).GreaterThan(0).When(x => x.ExpirationInMinutes.HasValue);
    }
} 