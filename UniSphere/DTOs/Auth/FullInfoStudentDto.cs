namespace UniSphere.Api.DTOs.Auth;


public record BaseStudentDto
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

public sealed record FullInfoStudentDto : BaseStudentDto
{
   
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
