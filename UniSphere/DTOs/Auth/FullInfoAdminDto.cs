namespace UniSphere.Api.DTOs.Auth;

public sealed record FullInfoAdminDto : BaseAdminDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
} 