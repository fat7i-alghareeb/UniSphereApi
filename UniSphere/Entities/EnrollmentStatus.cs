using System.ComponentModel.DataAnnotations;

namespace UniSphere.Api.Entities;

public class EnrollmentStatus
{
    public Guid Id { get; set; }
    public MultilingualText Name { get; set; } = new();
    public List<StudentCredential> StudentCredentials { get; set; } = new();
} 