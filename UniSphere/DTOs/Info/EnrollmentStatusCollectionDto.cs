using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Info;

public class EnrollmentStatusCollectionDto
{
    public List<EnrollmentStatus> EnrollmentStatuses { get; set; } = new();
} 