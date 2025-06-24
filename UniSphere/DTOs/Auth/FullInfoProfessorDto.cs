namespace UniSphere.Api.DTOs.Auth;

public sealed record FullInfoProfessorDto : BaseProfessorDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
} 