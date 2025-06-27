namespace UniSphere.Api.DTOs.Auth;

public record BaseStudentDto
{
    public required MultilingualNameDto FirstName { get; init; }
    public required MultilingualNameDto LastName { get; init; }
    public required MultilingualNameDto FatherName { get; init; }
    public required MultilingualNameDto EnrollmentStatusName { get; init; }
    public required int Year { get; init; }
    public required string StudentNumber { get; init; }
    public required MultilingualNameDto MajorName { get; init; }
    public required string StudentImageUrl { get; init; }
    public required Guid MajorId { get; init; }
    public required Guid StudentId { get; init; }
    public required int NumberOfMajorYears { get; init; }
    public required string Role { get; init; }
}

public sealed record FullInfoStudentDto : BaseStudentDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
