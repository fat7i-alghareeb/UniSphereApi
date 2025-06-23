namespace UniSphere.Api.DTOs.Auth;

public sealed record AdminCheckOneTimeCodeDto
{
    public required int Code { get; init; }
    public required string Gmail { get; init; }
    public required Guid MajorId { get; init; }
} 