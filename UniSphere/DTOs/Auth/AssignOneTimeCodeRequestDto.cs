namespace UniSphere.Api.DTOs.Auth;

public enum AssignOneTimeCodeTargetRole
{
    Admin,
    Professor,
    Student
}

public sealed record AssignOneTimeCodeRequestDto
{
    public required AssignOneTimeCodeTargetRole TargetRole { get; init; }
    public Guid? AdminId { get; init; }
    public Guid? ProfessorId { get; init; }
    public Guid? StudentId { get; init; }
    public int? ExpirationInMinutes { get; init; } // Optional, default 10
    public int? OneTimeCode { get; init; } // Optional, if not provided, generate randomly
} 
