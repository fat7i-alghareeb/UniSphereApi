namespace UniSphere.Api.DTOs.Announcements;

public sealed record  Top10FacultyAnnouncementsCollectionDto
{
    public required List<Top10FacultyAnnouncementsDto> Announcements { get; set; }
}
