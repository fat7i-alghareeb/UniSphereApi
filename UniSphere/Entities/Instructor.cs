using System.ComponentModel.DataAnnotations;

namespace UniSphere.Api.Entities;

public class Instructor
{
    public Guid Id { get; set; }
    public MultilingualText FirstName { get; set; } = new();
    public MultilingualText LastName { get; set; } = new();
    public MultilingualText? FatherName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Image { get; set; }
    
    public List<InstructorLabLink>? InstructorLabLinks { get; set; }
} 