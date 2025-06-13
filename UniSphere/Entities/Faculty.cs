using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSphere.Api.Entities;

public class Faculty
{
    public Guid Id { get; set; }
    public MultilingualText Name { get; set; } = new();
    public Guid UniversityId { get; set; }
    public University University { get; set; } = null!;
    public List<Major> Majors { get; set; } = new();
    public List<Schedule> Schedules { get; set; } = new();
    public List<FacultyAnnouncement>? FacultyAnnouncements { get; set; }
} 