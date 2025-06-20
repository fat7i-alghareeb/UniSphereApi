namespace UniSphere.Api.DTOs.Statistics;

public sealed record class StatisticsDto
{
    public double Average { get; init; }
    public required double NumberOfAttendanceHours { get; set; }
    public required int NumberOfAttendanceLectures { get; set; }
}
