namespace UniSphere.Api.DTOs.Auth;

public record BaseAdminDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Gmail { get; init; }
    public required string MajorName { get; init; }
    public required Guid MajorId { get; init; }
    public required Guid AdminId { get; init; }
} 