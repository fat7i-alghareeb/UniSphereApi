namespace UniSphere.Api.DTOs.Auth;

public sealed record SimpleStudentDto
{
    public required MultilingualNameDto FullName { get; init; }
    public required int Year { get; init; }
    public required string StudentNumber { get; init; }
    public required MultilingualNameDto MajorName { get; init; }
    public required Guid MajorId { get; init; }
    public required Guid StudentId { get; init; }
}
