namespace UniSphere.Api.DTOs.Subjects;

public sealed record CreateSubjectDto
{
    public required Guid Id { get; init; }
    public required Guid MajorId { get; init; }

}
