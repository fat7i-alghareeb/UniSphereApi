namespace UniSphere.Api.DTOs.Professors;

public sealed record AssignProfessorToSubjectDto
{
    public required Guid ProfessorId { get; init; }
    public required Guid SubjectId { get; init; }
} 