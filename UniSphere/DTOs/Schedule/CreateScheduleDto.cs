using System.ComponentModel.DataAnnotations;

namespace UniSphere.Api.DTOs.Schedule;

public sealed record CreateScheduleDto
{
    public required int Year { get; init; }
    public required DateOnly ScheduleDate { get; init; }
} 