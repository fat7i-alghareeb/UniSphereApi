namespace UniSphere.Api.DTOs.Subjects;

public sealed record UploadMaterialResponseDto
{
    public required string Url { get; init; }
    public required DateTime CreatedAt { get; init; }
} 