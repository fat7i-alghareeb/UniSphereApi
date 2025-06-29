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
            Title = lang == Languages.En ? announcement.Title.En ?? "" : announcement.Title.Ar ?? "",
            Description = lang == Languages.En ? announcement.Content.En ?? "" : announcement.Content.Ar ?? "",
            CreatedAt = announcement.CreatedAt
        };
    }
    public static Expression<Func<FacultyAnnouncement, FacultyAnnouncementsDto>> ProjectToFacultyAnnouncementsDto(Languages lang)
    {
        return announcement => new FacultyAnnouncementsDto
        {
            AnnouncementId = announcement.Id,
            Title = lang == Languages.En ? announcement.Title.En ?? "" : announcement.Title.Ar ?? "",
            Description = lang == Languages.En ? announcement.Content.En ?? "" : announcement.Content.Ar ?? "",
            CreatedAt = announcement.CreatedAt,
            Images = announcement.Images != null 
                ? announcement.Images.Select(img => new FacultyAnnouncementImageDto
                {
                    Id = img.Id,
                    Url = img.Url,
                    CreatedAt = img.CreatedAt
                }).ToList()
                : new List<FacultyAnnouncementImageDto>()
        };
    }    public static Expression<Func<FacultyAnnouncement, Top10FacultyAnnouncementsDto>>  ProjectToTop10FacultyAnnouncementsDto(Languages lang)
    {
        return announcement => new Top10FacultyAnnouncementsDto
        {
            AnnouncementId = announcement.Id,
            Image = "",
            Description = lang == Languages.En ? announcement.Content.En ?? announcement.Title.Ar ?? "" : announcement.Content.Ar ?? announcement.Title.En ?? "",
            // Title = lang == Languages.En ? announcement.Title.En : announcement.Title.Ar
        };
    }
}
