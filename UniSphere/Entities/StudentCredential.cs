
namespace UniSphere.Api.Entities;

public class StudentCredential
{
    public Guid Id { get; set; }
    public string StudentNumber { get; set; }
    public string? IdentityId { get; set; }
    public Guid MajorId { get; set; }
    public int? OneTimeCode { get; set; }
    public DateTime? OneTimeCodeCreatedDate { get; set; }
    public int? OneTimeCodeExpirationInMinutes { get; init; }

    public MultilingualText FirstName { get; set; } = new();
    public MultilingualText LastName { get; set; } = new();
    public MultilingualText? FatherName { get; set; }
    public int Year { get; set; }
    public Guid EnrollmentStatusId { get; set; }
    public string? Image { get; set; }
    
    
    // Navigation properties
    public Major Major { get; set; } = null!;
    public EnrollmentStatus EnrollmentStatus { get; set; } = null!;
    public List<SubjectStudentLink>? SubjectStudentLinks { get; set; }
} 
