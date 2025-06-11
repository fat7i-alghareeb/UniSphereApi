using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniSphere.Api.Entities;

public class Schedule
{
    public Guid Id { get; set; }
    public Guid MajorId { get; set; }
    public Guid FacultyId { get; set; }
    public DateOnly DayDate { get; set; }
    public int Year { get; set; }
    public int Semester { get; set; }
    
    public Major Major { get; set; } = null!;
    public Faculty Faculty { get; set; } = null!;
    public List<Lecture> Lectures { get; set; } = new();
} 