namespace UniSphere.Api.Entities;

public class ScheduleLabLink
{
    public required Guid ScheduleId { get; set; }
    public required Guid LabId { get; set; }
    public required TimeSpan StartTime { get; set; }
    public required TimeSpan EndTime { get; set; }
    public required Schedule Schedule { get; set; }
    public required Lab Lab { get; set; }
}
