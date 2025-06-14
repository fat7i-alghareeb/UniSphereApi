namespace UniSphere.Api.DTOs.Auth;

public sealed record  FullInfoStudentDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string FatherName { get; init; }
    public required string EnrollmentStatusName { get; init; }
    public required int Year { get; init; }
    public required string StudentNumber { get; init; }
    public required string MajorName { get; init; }
    public required string StudentImageUrl { get; init; }
    public required Guid MajorId { get; init; }
    public required Guid StudentId { get; init; }
}