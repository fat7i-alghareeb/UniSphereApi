namespace UniSphere.Api.DTOs.Schedule;

public sealed record CreateLectureDto
{
    public Guid? Id { get; init; } // Optional for creation, required for patching
    public required string SubjectNameEn { get; init; }
    public required string SubjectNameAr { get; init; }
    public required string LecturerNameEn { get; init; }
    public required string LecturerNameAr { get; init; }
    public required TimeSpan StartTime { get; init; }
    public required TimeSpan EndTime { get; init; }
    public required string LectureHallEn { get; init; }
    public required string LectureHallAr { get; init; }
} 