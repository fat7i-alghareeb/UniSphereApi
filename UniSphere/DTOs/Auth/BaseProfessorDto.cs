namespace UniSphere.Api.DTOs.Auth;

public record BaseProfessorDto
{
    public required MultilingualNameDto FirstName { get; init; }
    public required MultilingualNameDto LastName { get; init; }
    public required string Gmail { get; init; }
    public required MultilingualTextDto Bio { get; init; }
    public required string Image { get; init; }
    public required Guid ProfessorId { get; init; }
    public required string Role { get; init; } = "Professor";
} 