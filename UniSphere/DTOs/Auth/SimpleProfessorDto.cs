namespace UniSphere.Api.DTOs.Auth;

public sealed record SimpleProfessorDto
{
    public required MultilingualNameDto FullName { get; init; }
    public required string Gmail { get; init; }
    public required Guid ProfessorId { get; init; }
} 