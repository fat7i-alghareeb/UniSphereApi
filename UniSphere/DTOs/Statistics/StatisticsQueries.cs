using System.Linq.Expressions;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Statistics;

internal static  class StatisticsQueries
{
    public static Expression<Func<StudentStatistics, StatisticsDto>> ProjectToDto( double average) =>
        statistics => new StatisticsDto
        {
            Average = average,
            NumberOfAttendanceHours = statistics.NumberOfAttendanceHours,
            NumberOfAttendanceLectures = statistics.NumberOfAttendanceLectures
        };
}
