using UniSphere.Api.DTOs.Subjects;

namespace UniSphere.Api.DTOs.Info;

public class SubjectNameIdCollectionDto
{
    public List<SubjectNameIdDto> Subjects { get; set; } = new();
} 