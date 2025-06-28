namespace UniSphere.Api.DTOs.Schedule;

public sealed record class DayLectureDto
{
    public required Guid Id { get; init; }
    public required string SubjectName { get; init; }
    public required string LectureName { get; init; }
    public required string LectureHall { get; init; }
    public required TimeSpan StartTime { get; init; }
    public required TimeSpan EndTime { get; init; }
}
