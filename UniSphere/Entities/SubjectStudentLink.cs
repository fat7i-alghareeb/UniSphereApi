using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSphere.Api.Entities;

public class SubjectStudentLink
{
    public required Guid SubjectId { get; set; }
    public required Guid StudentId { get; set; }
    public required int AttemptNumber { get; set; }
    public int? MidtermGrade { get; set; }
    public int? FinalGrade { get; set; }
    public required bool IsCurrentlyEnrolled { get; set; }
    public required bool IsPassed { get; set; }
    public MultilingualText? Notes { get; set; }
    
    public Subject Subject { get; set; }
    public StudentCredential StudentCredential { get; set; }
} 
