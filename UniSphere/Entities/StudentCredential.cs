using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace UniSphere.Api.Entities;

public class StudentCredential
{
    public Guid Id { get; set; }
    public Guid FacultyId { get; set; }
    public Guid MajorId { get; set; }
    public string Email { get; set; } = string.Empty;
    public int? OneTimeCode { get; set; }
    public MultilingualText FirstName { get; set; } = new();
    public MultilingualText LastName { get; set; } = new();
    public MultilingualText? FatherName { get; set; }
    public int Year { get; set; }
    public Guid EnrollmentStatusId { get; set; }
    public string? Image { get; set; }
    
    // Navigation properties
    public Major Major { get; set; } = null!;
    public EnrollmentStatus EnrollmentStatus { get; set; } = null!;
    public List<SubjectStudentLink> SubjectStudentLinks { get; set; } = new();
} 