namespace UniSphere.Api.DTOs.Auth;

public sealed record class LoginProfessorDto
{
    public required string Gmail { get; init; }
    public required string Password { get; init; }
} 