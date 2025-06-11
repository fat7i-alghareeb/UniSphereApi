using System.ComponentModel.DataAnnotations;

namespace UniSphere.Api.Entities;

public class Instructor
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Image { get; set; }
    public List<InstructorLab> InstructorLabs { get; set; } = new();
} 