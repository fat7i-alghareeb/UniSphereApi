using System.ComponentModel.DataAnnotations;

namespace UniSphere.Api.Entities;

public class Lab
{
    public Guid Id { get; set; }
    public MultilingualText Name { get; set; } = new();
    public MultilingualText? Description { get; set; }
    public string? Image { get; set; }
    public List<InstructorLabLink> InstructorLabLinks { get; set; } = new();
    public List<Subject> Subjects { get; set; } = new();
} 