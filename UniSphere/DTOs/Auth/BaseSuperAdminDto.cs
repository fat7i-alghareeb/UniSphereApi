namespace UniSphere.Api.DTOs.Auth;

public record BaseSuperAdminDto
{
    public required MultilingualNameDto FirstName { get; init; }
    public required MultilingualNameDto LastName { get; init; }
    public required string Gmail { get; init; }
    public required MultilingualNameDto FacultyName { get; init; }
    public required Guid FacultyId { get; init; }
    public required Guid SuperAdminId { get; init; }
    public required string Role { get; init; } = "SuperAdmin";
    public string? Image { get; init; }
} 