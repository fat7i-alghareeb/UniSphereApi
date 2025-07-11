namespace UniSphere.Api.DTOs.Info;

public sealed record AdminIdNameDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
} 