using System.ComponentModel.DataAnnotations;

namespace UniSphere.Api.DTOs.Schedule;

public sealed record CreateScheduleDto
{
    public required Guid MajorId { get; init; }
    public required int Year { get; init; }
    public required DateOnly ScheduleDate { get; init; }
    public required List<CreateLectureDto> Lectures { get; init; }
}

public sealed record CreateLectureDto
{
    public required string SubjectNameEn { get; init; }
    public required string SubjectNameAr { get; init; }
    public required string LecturerNameEn { get; init; }
    public required string LecturerNameAr { get; init; }
    public required TimeSpan StartTime { get; init; }
    public required TimeSpan EndTime { get; init; }
    public required string LectureHallEn { get; init; }
    public required string LectureHallAr { get; init; }
} 