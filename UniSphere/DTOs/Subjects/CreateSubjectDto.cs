using FluentValidation;

namespace UniSphere.Api.DTOs.Subjects;

public sealed record CreateSubjectDto
{
    public required Guid MajorId { get; init; }
    public required string Name { get; init; } = string.Empty;

}
public class CreateSubjectDtoValidator : AbstractValidator<CreateSubjectDto>
{
    public CreateSubjectDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");
        RuleFor(x => x.MajorId) 
            .NotEmpty().WithMessage("MajorId is required.");
    }
}
