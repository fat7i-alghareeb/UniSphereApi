namespace UniSphere.Api.DTOs.Professors;

public sealed record RemoveProfessorFromFacultyDto
{
    public required Guid ProfessorId { get; init; }
} 