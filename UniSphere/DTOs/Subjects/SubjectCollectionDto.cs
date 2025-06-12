namespace UniSphere.Api.DTOs.Subjects;

public sealed record SubjectCollectionDto
{
    public List<SubjectDto> Subjects { get; init; }
}
