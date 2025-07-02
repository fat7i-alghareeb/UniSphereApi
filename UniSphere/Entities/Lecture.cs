namespace UniSphere.Api.Entities;

public class Lecture
{
    public Guid Id { get; set; }
    public Guid ScheduleId { get; set; }
    public Guid SubjectId { get; set; }
    public Guid ProfessorId { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public MultilingualText LectureHall { get; set; } = new();
    
    // Navigation properties
    public Schedule Schedule { get; set; } = null!;
    public Subject Subject { get; set; } = null!;
    public Professor Professor { get; set; } = null!;
} 