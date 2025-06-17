namespace UniSphere.Api.DTOs.Subjects;

public sealed record SubjectDto
{
    public required Guid Id { get; init; }
    public required Guid MajorId { get; init; }
    public required int Year { get; init; }
    public required int Semester { get; init; }
    public required string MajoreName { get; init; }
}


