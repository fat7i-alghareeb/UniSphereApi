using System.Linq.Expressions;
using UniSphere.Api.Controllers;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Schedule;

internal static class ScheduleQueries
{
    public static Expression<Func<Entities.Schedule, MonthScheduleDto>> ProjectToMonthScheduleDto(DateOnly month,Languages lang)
    {
        return schedule => schedule.ToMonthScheduleDto(month,lang);
    }

    public static Expression<Func<List<Entities.Schedule>, MonthScheduleDto>> ProjectToCombinedMonthScheduleDto(DateOnly month,Languages lang)
    {
        return schedules => schedules.CombineSchedulesIntoMonth(month,lang);
    }
}
