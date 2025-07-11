namespace UniSphere.Api.DTOs.Auth;

public sealed record IdNameDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
} 