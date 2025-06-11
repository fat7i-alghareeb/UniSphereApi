using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace UniSphere.Api.Entities;

public class StudentCredential
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public Guid FacultyId { get; set; }
    
    [Required]
    public Guid MajorId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;
    
    public int? OneTimeCode { get; set; }
    
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? FatherName { get; set; }
    
    [Required]
    public int Year { get; set; }
    
    [Required]
    public Guid EnrollmentStatusId { get; set; }
    
    public string? Image { get; set; }
    
    // Navigation properties
    [ForeignKey(nameof(FacultyId))]
    public Faculty Faculty { get; set; } = null!;
    
    [ForeignKey(nameof(MajorId))]
    public Major Major { get; set; } = null!;
    
    [ForeignKey(nameof(EnrollmentStatusId))]
    public EnrollmentStatus EnrollmentStatus { get; set; } = null!;
    
    public ICollection<SubjectStudentLink> SubjectStudentLinks { get; set; } = new List<SubjectStudentLink>();
} 