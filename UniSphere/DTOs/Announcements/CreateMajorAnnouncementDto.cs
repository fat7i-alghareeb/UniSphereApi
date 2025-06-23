using System.ComponentModel.DataAnnotations;

namespace UniSphere.Api.DTOs.Announcements;

public sealed record CreateMajorAnnouncementDto
{
    public required Guid MajorId { get; init; }
    public required Guid SubjectId { get; init; }
    public required int Year { get; init; }
    public required string TitleEn { get; init; }
    public required string TitleAr { get; init; }
    public required string ContentEn { get; init; }
    public required string ContentAr { get; init; }
} 