namespace UniSphere.Api.DTOs.Auth;

public sealed record SuperAdminCheckOneTimeCodeDto
{
    public required int Code { get; init; }
    public required string Gmail { get; init; }
    public required Guid FacultyId { get; init; }
} 