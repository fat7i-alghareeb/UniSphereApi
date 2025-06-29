namespace UniSphere.Api.DTOs.Announcements;

public sealed record FacultyAnnouncementImageDto
{
    public required Guid Id { get; init; }
    public required string Url { get; init; }
    public required DateTime CreatedAt { get; init; }
} 