namespace UniSphere.Api.DTOs.Info;

public sealed record MajorNameDto
{
    public required Guid Id { get; init; }
    public required  string Name { get; init; }
    public required int NumberOfYears { get; init; }
}


