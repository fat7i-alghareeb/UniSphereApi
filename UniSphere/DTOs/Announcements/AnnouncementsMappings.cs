using UniSphere.Api.Controllers;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Announcements;

internal static class AnnouncementsMappings
{

    public static StudentAnnouncementsDto
        ToStudentAnnouncementsDto(this MajorAnnouncement announcement, Languages lang) => new StudentAnnouncementsDto
    {
        AnnouncementId = announcement.Id,
        Title = announcement.Title.GetTranslatedString(lang),
        Description = announcement.Content.GetTranslatedString(lang),
        CreatedAt = announcement.CreatedAt
    };

    public static FacultyAnnouncementsDto
        ToFacultyAnnouncementsDto(this FacultyAnnouncement announcement, Languages lang) => new FacultyAnnouncementsDto
    {
        AnnouncementId = announcement.Id,
        Title = announcement.Title.GetTranslatedString(lang),
        Description = announcement.Content.GetTranslatedString(lang),
        CreatedAt = announcement.CreatedAt,
        Images = []
    };

}
