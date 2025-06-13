using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSphere.Api.Entities;

public class Major
{
    public Guid Id { get; set; }
    public MultilingualText Name { get; set; } = new();
    public Guid FacultyId { get; set; }
    public int NumberOfYears { get; set; }
    public Faculty Faculty { get; set; } = null!;
    public List<Subject> Subjects { get; set; } = new();
    public List<StudentCredential> StudentCredentials { get; set; } = new();
    public List<Schedule> Schedules { get; set; } = new();
    public ICollection<MajorAnnouncement> MajorAnnouncements { get; set; }
} 