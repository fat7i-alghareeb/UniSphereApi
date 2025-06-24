
namespace UniSphere.Api.Entities;

public class StudentCredential
{
    public required Guid Id { get; set; }
    public required string StudentNumber { get; set; }
    public string? IdentityId { get; set; }
    public required Guid MajorId { get; set; }
    public int? OneTimeCode { get; set; }
    public DateTime? OneTimeCodeCreatedDate { get; set; }
    public int? OneTimeCodeExpirationInMinutes { get; set; }

    public required MultilingualText FirstName { get; set; } = new();
    public required MultilingualText LastName { get; set; } = new();
    public MultilingualText? FatherName { get; set; }
    public required int Year { get; set; }
    public required Guid EnrollmentStatusId { get; set; }
    public string? Image { get; set; }
    
    
    // Navigation properties
    public Major Major { get; set; } = null!;
    public StudentStatistics StudentStatistics { get; set; } = null!;
    public EnrollmentStatus EnrollmentStatus { get; set; } = null!;
    public List<SubjectStudentLink>? SubjectStudentLinks { get; set; }
} 
