namespace UniSphere.Api.DTOs.Auth;

public record BaseProfessorDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Gmail { get; init; }
    public required string Bio { get; init; }
    public required string Image { get; init; }
    public required Guid ProfessorId { get; init; }
} 