using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSphere.Api.Entities; 

public class StudentStatistics{
    public required Guid Id { get; set; }
    public required Guid StudentId { get; set; }
    public required double NumberOfAttendanceHours { get; set; }
    public required int NumberOfAttendanceLectures { get; set; }
    public StudentCredential Student { get; set; } = null!;
}
