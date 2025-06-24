namespace UniSphere.Api.DTOs.Auth;

public record BaseSystemControllerDto
{
    public required string Gmail { get; init; }
    public required string UserName { get; init; }
    public required Guid SystemControllerId { get; init; }
} 