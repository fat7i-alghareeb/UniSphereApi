namespace UniSphere.Api.DTOs.Announcements;

public sealed record  FacultyAnnouncementsDto
{
    public required Guid AnnouncementId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required List<string>? Images { get; set; }
    public required DateTime? CreatedAt { get; set; }
}
