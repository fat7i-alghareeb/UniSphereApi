namespace UniSphere.Api.DTOs.Auth;

public sealed record  SimpleStudentDto
{
    public required string FullName { get; init; }
    public required int Year { get; init; }
    public required string StudentNumber { get; init; }
    public required string MajorName { get; init; }
    public required Guid MajorId { get; init; }
    public required Guid StudentId { get; init; }
}
