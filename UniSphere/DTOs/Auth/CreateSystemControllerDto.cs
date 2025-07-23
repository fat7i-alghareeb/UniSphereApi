namespace UniSphere.Api.DTOs.Auth;

public sealed record CreateSystemControllerDto
{
    public required string Gmail { get; init; }
    public required string UserName { get; init; }
    public required string Password { get; init; }
}
