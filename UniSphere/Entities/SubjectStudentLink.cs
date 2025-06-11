using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSphere.Api.Entities;

public class SubjectStudentLink
{
    public Guid SubjectId { get; set; }
    public Guid StudentId { get; set; }
    public Guid FacultyId { get; set; }
    public int AttemptNumber { get; set; }
    public int? MidtermGrade { get; set; }
    public int? FinalGrade { get; set; }
    public bool IsCurrentlyEnrolled { get; set; }
    public bool? IsPassed { get; set; }
    public MultilingualText? Notes { get; set; }
    
    public Subject Subject { get; set; } = null!;
    public StudentCredential StudentCredential { get; set; } = null!;
} 