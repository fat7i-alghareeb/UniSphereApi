using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSphere.Api.Entities;

public class Schedule
{
    public Guid Id { get; set; }
    public Guid MajorId { get; set; }
    public int Year { get; set; }
    public DateOnly ScheduleDate { get; set; }
    public Major Major { get; set; } = null!;
    public List<Lecture> Lectures { get; set; } = new();
    public List<ScheduleLabLink> ScheduleLabLinks { get; set; } = new();
} 
