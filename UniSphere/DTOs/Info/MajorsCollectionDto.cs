namespace UniSphere.Api.DTOs.Info;

public sealed record MajorsCollectionDto
{
    public List<MajorNameDto> Majors { get; init; }
}
