namespace UniSphere.Api.DTOs.Auth;

public sealed record CreateSuperAdminDto
{
    public required string FirstNameEn { get; init; }
    public required string FirstNameAr { get; init; }
    public required string LastNameEn { get; init; }
    public required string LastNameAr { get; init; }
    public required string Gmail { get; init; }
    public required Guid FacultyId { get; init; }
} 