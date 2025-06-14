namespace UniSphere.Api.DTOs.Auth;

public sealed record RegisterStudentDto
{
    public required string UserName { get; init; }
    public required Guid StudentId { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }
}