namespace UniSphere.Api.DTOs.Schedule;

public sealed record class DayLectureDto
{
    public required Guid Id { get; init; }
    public required string SubjectName { get; init; }
    public required string ProfessorName { get; init; }
    public required string LectureHall { get; init; }
    public required TimeSpan StartTime { get; init; }
    public required TimeSpan EndTime { get; init; }
    public required Guid SubjectId { get; init; }
    public required Guid ProfessorId { get; init; }
}
