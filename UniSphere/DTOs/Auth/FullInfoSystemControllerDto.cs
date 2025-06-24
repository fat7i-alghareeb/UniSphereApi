namespace UniSphere.Api.DTOs.Auth;

public sealed record FullInfoSystemControllerDto : BaseSystemControllerDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
} 