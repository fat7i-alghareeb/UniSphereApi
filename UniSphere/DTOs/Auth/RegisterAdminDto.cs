namespace UniSphere.Api.DTOs.Auth;

public sealed record RegisterAdminDto
{
    public required string UserName { get; init; }
    public required Guid AdminId { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }
} 