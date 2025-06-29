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

    public static void UpdateFromDto(this MajorAnnouncement announcement, StudentAnnouncementsDto dto)
    {
        announcement.Title = new MultilingualText { En = dto.Title, Ar = dto.Title };
        announcement.Content = new MultilingualText { En = dto.Description, Ar = dto.Description };
    }

    public static void UpdateFromDto(this FacultyAnnouncement announcement, FacultyAnnouncementsDto dto)
    {
        announcement.Title = new MultilingualText { En = dto.Title, Ar = dto.Title };
        announcement.Content = new MultilingualText { En = dto.Description, Ar = dto.Description };
    }

    public static FacultyAnnouncement ToFacultyAnnouncement(this CreateFacultyAnnouncementDto dto, Guid facultyId)
    {
        return new FacultyAnnouncement
        {
            Id = Guid.NewGuid(),
            FacultyId = facultyId,
            Title = new MultilingualText { En = dto.TitleEn, Ar = dto.TitleAr },
            Content = new MultilingualText { En = dto.ContentEn, Ar = dto.ContentAr },
            CreatedAt = DateTime.UtcNow
        };
    }

    public static FacultyAnnouncement ToFacultyAnnouncementWithImages(this CreateFacultyAnnouncementWithImagesDto dto, Guid facultyId)
    {
        return new FacultyAnnouncement
        {
            Id = Guid.NewGuid(),
            FacultyId = facultyId,
            Title = new MultilingualText { En = dto.TitleEn, Ar = dto.TitleAr },
            Content = new MultilingualText { En = dto.ContentEn, Ar = dto.ContentAr },
            CreatedAt = DateTime.UtcNow,
            Images = new List<FacultyAnnouncementImage>()
        };
    }

    public static FacultyAnnouncementImageDto ToImageDto(this FacultyAnnouncementImage image)
    {
        return new FacultyAnnouncementImageDto
        {
            Id = image.Id,
            Url = image.Url,
            CreatedAt = image.CreatedAt
        };
    }

    public static MajorAnnouncement ToMajorAnnouncement(this CreateMajorAnnouncementDto dto, Guid majorId)
    {
        return new MajorAnnouncement
        {
            Id = Guid.NewGuid(),
            MajorId = majorId,
            SubjectId = dto.SubjectId,
            Year = dto.Year,
            Title = new MultilingualText { En = dto.TitleEn, Ar = dto.TitleAr },
            Content = new MultilingualText { En = dto.ContentEn, Ar = dto.ContentAr },
            CreatedAt = DateTime.UtcNow
        };
    }

}
