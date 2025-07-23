using UniSphere.Api.DTOs.Auth;

namespace UniSphere.Api.DTOs.Info;

public class ProfessorIdNameCollectionDto
{
    public List<ProfessorIdNameDto> Professors { get; set; } = new();
} 