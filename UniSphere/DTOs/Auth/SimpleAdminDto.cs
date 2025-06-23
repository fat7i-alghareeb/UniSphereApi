namespace UniSphere.Api.DTOs.Auth;

public sealed record SimpleAdminDto
{
    public required string FullName { get; init; }
    public required string Gmail { get; init; }
    public required string MajorName { get; init; }
    public required Guid MajorId { get; init; }
    public required Guid AdminId { get; init; }
} 