namespace UniSphere.Api.DTOs.Auth;

public sealed record CreateEnrollmentStatusDto
{
    public required string NameEn { get; init; }
    public required string NameAr { get; init; }
} 