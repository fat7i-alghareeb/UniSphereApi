namespace UniSphere.Api.DTOs.Auth;

public sealed record SimpleSystemControllerDto
{
    public required string FullName { get; init; }
    public required string Gmail { get; init; }
    public required string UserName { get; init; }
    public required Guid SystemControllerId { get; init; }
} 