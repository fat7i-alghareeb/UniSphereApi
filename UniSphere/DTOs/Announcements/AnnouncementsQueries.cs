using System.Linq.Expressions;
using UniSphere.Api.Controllers;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Announcements;

public static class AnnouncementsQueries
{
    public static Expression<Func<MajorAnnouncement, StudentAnnouncementsDto>> ProjectToStudentAnnouncementsDto(Languages lang)
    {
        return announcement => new StudentAnnouncementsDto
        {
            AnnouncementId = announcement.Id,
            Title = announcement.Title.GetTranslatedString(lang),
            Description = announcement.Content.GetTranslatedString(lang),
            CreatedAt = announcement.CreatedAt
        };
    }
    public static Expression<Func<FacultyAnnouncement, FacultyAnnouncementsDto>> ProjectToFacultyAnnouncementsDto(Languages lang)
    {
        return announcement => announcement.ToFacultyAnnouncementsDto(lang);
    }
}
