namespace UniSphere.Api.DTOs.Professors;

public sealed record AddProfessorToFacultyDto
{
    public required Guid ProfessorId { get; init; }
} 