namespace UniSphere.Api.DTOs.Auth;

public sealed record AssignSuperAdminOneTimeCodeDto
{
    public required Guid SuperAdminId { get; init; }
    public int? ExpirationInMinutes { get; init; } // Optional, default 10
} 