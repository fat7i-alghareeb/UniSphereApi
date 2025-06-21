namespace UniSphere.Api.DTOs.Announcements;

public sealed record  StudentAnnouncementsDto
{
    public required Guid AnnouncementId { get; set; }
    public required string? Title { get; set; }
    public required string? Description { get; set; }
    public required DateTime? CreatedAt { get; set; }
}
