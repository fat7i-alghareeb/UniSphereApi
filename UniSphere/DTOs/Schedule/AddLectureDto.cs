using System.ComponentModel.DataAnnotations;

namespace UniSphere.Api.DTOs.Schedule;

public sealed record AddLectureDto
{
    public required Guid ScheduleId { get; init; }
    public required string SubjectNameEn { get; init; }
    public required string SubjectNameAr { get; init; }
    public required string LecturerNameEn { get; init; }
    public required string LecturerNameAr { get; init; }
    public required TimeSpan StartTime { get; init; }
    public required TimeSpan EndTime { get; init; }
    public required string LectureHallEn { get; init; }
    public required string LectureHallAr { get; init; }
} 