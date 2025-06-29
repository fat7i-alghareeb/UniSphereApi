namespace UniSphere.Api.Entities;

public class FacultyAnnouncementImage
{
    public required Guid Id { get; set; }
    public required Guid FacultyAnnouncementId { get; set; }
    public required string Url { get; set; }
    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public FacultyAnnouncement FacultyAnnouncement { get; set; } = null!;
} 