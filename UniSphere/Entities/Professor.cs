using System.ComponentModel.DataAnnotations;

namespace UniSphere.Api.Entities;

public class Professor
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Image { get; set; }
    public List<SubjectLecturer> SubjectLecturers { get; set; } = new();
} 