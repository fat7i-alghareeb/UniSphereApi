namespace UniSphere.Api.DTOs.Subjects;

public sealed record SubjectDto
{
    public required Guid Id { get; init; }
    public required Guid MajorId { get; init; }
}


