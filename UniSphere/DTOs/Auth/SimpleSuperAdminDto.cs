namespace UniSphere.Api.DTOs.Auth;

public sealed record SimpleSuperAdminDto
{
    public required MultilingualNameDto FullName { get; init; }
    public required string Gmail { get; init; }
    public required MultilingualNameDto FacultyName { get; init; }
    public required Guid FacultyId { get; init; }
    public required Guid SuperAdminId { get; init; }
} 