namespace UniSphere.Api.DTOs.Auth;

public sealed record SimpleAdminDto
{
    public required MultilingualNameDto FullName { get; init; }
    public required string Gmail { get; init; }
    public required MultilingualNameDto MajorName { get; init; }
    public required Guid MajorId { get; init; }
    public required Guid AdminId { get; init; }
} 