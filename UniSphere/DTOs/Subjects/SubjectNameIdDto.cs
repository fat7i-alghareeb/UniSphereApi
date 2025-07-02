namespace UniSphere.Api.DTOs.Subjects;

public sealed record SubjectNameIdDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
} 