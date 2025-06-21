namespace UniSphere.Api.DTOs.Announcements;

public sealed record  FacultyAnnouncementsCollectionDto
{
    public required List<FacultyAnnouncementsDto> Announcements { get; set; }
}
