namespace UniSphere.Api.DTOs.Auth;

public sealed record class LoginSuperAdminDto
{
    public required string Gmail { get; init; }
    public required Guid FacultyId { get; init; }
    public required string Password { get; init; }
} 