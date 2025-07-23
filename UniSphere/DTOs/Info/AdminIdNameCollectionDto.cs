using UniSphere.Api.DTOs.Auth;

namespace UniSphere.Api.DTOs.Info;

public class AdminIdNameCollectionDto
{
    public List<AdminIdNameDto> Admins { get; set; } = new();
} 