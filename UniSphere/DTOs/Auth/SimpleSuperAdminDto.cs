namespace UniSphere.Api.DTOs.Auth;

public sealed record SimpleSuperAdminDto
{
    public required string FullName { get; init; }
    public required string Gmail { get; init; }
    public required string FacultyName { get; init; }
    public required Guid FacultyId { get; init; }
    public required Guid SuperAdminId { get; init; }
} 