namespace UniSphere.Api.DTOs.Auth;

public record MultilingualTextDto
{
    public required string En { get; init; }
    public required string Ar { get; init; }
}

public record MultilingualNameDto
{
    public required string En { get; init; }
    public required string Ar { get; init; }
} 