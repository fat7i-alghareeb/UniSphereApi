using System.ComponentModel.DataAnnotations;

namespace UniSphere.Api.DTOs.Announcements;

public sealed record CreateFacultyAnnouncementDto
{
    public required Guid FacultyId { get; init; }
    public required string TitleEn { get; init; }
    public required string TitleAr { get; init; }
    public required string ContentEn { get; init; }
    public required string ContentAr { get; init; }
} 