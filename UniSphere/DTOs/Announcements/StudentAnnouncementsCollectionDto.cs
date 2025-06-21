namespace UniSphere.Api.DTOs.Announcements;

public sealed record  StudentAnnouncementsCollectionDto
{
    public required List<StudentAnnouncementsDto> Announcements { get; set; }
}
