namespace UniSphere.Api.Entities;

public class Lecture
{
    public Guid Id { get; set; }
    public Guid ScheduleId { get; set; }
    public MultilingualText SubjectName { get; set; } = new();
    public MultilingualText LecturerName { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public MultilingualText LectureHall { get; set; } = new();
    
    // Navigation property
    public Schedule Schedule { get; set; } = null!;
} 