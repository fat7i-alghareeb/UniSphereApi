using System.Linq.Expressions;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Schedule;

internal static class ScheduleQueries
{
    public static Expression<Func<Entities.Schedule, MonthScheduleDto>> ProjectToMonthScheduleDto(DateOnly month)
    {
        return schedule => schedule.ToMonthScheduleDto(month);
    }

    public static Expression<Func<List<Entities.Schedule>, MonthScheduleDto>> ProjectToCombinedMonthScheduleDto(DateOnly month)
    {
        return schedules => schedules.CombineSchedulesIntoMonth(month);
    }
}
