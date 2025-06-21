namespace UniSphere.Api.DTOs.Announcements;

public sealed record  Top10FacultyAnnouncementsDto
{
    public required Guid AnnouncementId { get; set; }
    public required string Image { get; set; }
    public required string Description { get; set; }

    
}
