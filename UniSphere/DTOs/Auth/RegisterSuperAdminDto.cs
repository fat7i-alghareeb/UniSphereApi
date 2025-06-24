namespace UniSphere.Api.DTOs.Auth;

public sealed record RegisterSuperAdminDto
{
    public required string UserName { get; init; }
    public required Guid SuperAdminId { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }
} 