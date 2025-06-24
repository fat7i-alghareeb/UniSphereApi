namespace UniSphere.Api.DTOs.Auth;

public sealed record RegisterSystemControllerDto
{
    public required string UserName { get; init; }
    public required Guid SystemControllerId { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }
} 