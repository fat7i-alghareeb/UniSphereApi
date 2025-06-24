namespace UniSphere.Api.DTOs.Auth;

public sealed record ProfessorCheckOneTimeCodeDto
{
    public required int Code { get; init; }
    public required string Gmail { get; init; }
} 