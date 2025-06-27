namespace UniSphere.Api.DTOs.Auth;

public record BaseAdminDto
{
    public required MultilingualNameDto FirstName { get; init; }
    public required MultilingualNameDto LastName { get; init; }
    public required string Gmail { get; init; }
    public required MultilingualNameDto MajorName { get; init; }
    public required Guid MajorId { get; init; }
    public required Guid AdminId { get; init; }
    public required string Role { get; init; } = "Admin";
    public string? Image { get; init; }
} 