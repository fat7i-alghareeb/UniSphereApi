namespace UniSphere.Api.DTOs.Auth;

public sealed record RegisterProfessorDto
{
    public required string UserName { get; init; }
    public required Guid ProfessorId { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }
} 