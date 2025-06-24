namespace UniSphere.Api.DTOs.Auth;

public sealed record CreateStudentDto
{
    public required string FirstNameEn { get; init; }
    public required string FirstNameAr { get; init; }
    public required string LastNameEn { get; init; }
    public required string LastNameAr { get; init; }
    public required string StudentNumber { get; init; }
    public required Guid MajorId { get; init; }
    public int Year { get; init; } = 1;
} 