namespace UniSphere.Api.DTOs.Auth;

public sealed record FullInfoSuperAdminDto : BaseSuperAdminDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
} 