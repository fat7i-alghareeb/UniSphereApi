namespace UniSphere.Api.DTOs.Auth;

public sealed record CheckOneTimeCodeDto
{
    
    public required int Code { get; init; }
    public required string StudentNumber { get; init; }
    public required  Guid MajorId { get; init; }
    
}
