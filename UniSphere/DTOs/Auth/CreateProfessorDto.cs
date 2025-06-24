namespace UniSphere.Api.DTOs.Auth;

public sealed record CreateProfessorDto
{
    public required string FirstNameEn { get; init; }
    public required string FirstNameAr { get; init; }
    public required string LastNameEn { get; init; }
    public required string LastNameAr { get; init; }
    public required string Gmail { get; init; }
    public string? BioEn { get; init; }
    public string? BioAr { get; init; }
} 