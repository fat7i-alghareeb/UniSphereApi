namespace UniSphere.Api.DTOs.Schedule;

public sealed record MonthScheduleDto
{
     public required DateOnly Month { get; init; }
     public required List<DayScheduleDto> Days { get; init; }
}
