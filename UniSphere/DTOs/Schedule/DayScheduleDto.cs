namespace UniSphere.Api.DTOs.Schedule;

public sealed record  DayScheduleDto
{
    public required DateOnly Date { get; init; }
    public required Guid ScheduleId { get; init; }
    public required List<DayLectureDto> Lectures { get; init; }
}
