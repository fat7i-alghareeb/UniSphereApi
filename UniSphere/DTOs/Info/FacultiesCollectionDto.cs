namespace UniSphere.Api.DTOs.Info;

public sealed record   FacultiesCollectionDto
{
    public List<FacultyNameDto> Factories { get; init; }
}
