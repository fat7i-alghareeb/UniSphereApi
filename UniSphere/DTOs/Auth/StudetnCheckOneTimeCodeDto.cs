namespace UniSphere.Api.DTOs.Auth;

public sealed record StudentCheckOneTimeCodeDto
{
    
    public required int Code { get; init; }
    public required string StudentNumber { get; init; }
    public required  Guid MajorId { get; init; }
    
}
