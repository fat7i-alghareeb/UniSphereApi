using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;

namespace UniSphere.Api.Entities;

public class Subject
{
    public required Guid Id { get; set; }
    public required MultilingualText Name { get; set; } = new();
    public required Guid MajorId { get; set; }
    public Guid? LabId { get; set; }
    public required MultilingualText Description { get; set; } = new();
    public required int Year { get; set; }
    public required int Semester { get; set; }
    public double MidtermGrade { get; set; } = 30;
    public double FinalGrade { get; set; } = 70;
    public double PassGrade { get; set; } = 70;
    public bool IsLabRequired { get; set; }
    public bool IsMultipleChoice { get; set; }
    public bool IsOpenBook { get; set; }
    public string? Image { get; set; }
        
    public Major Major { get; set; } = null!;
    public Lab? Lab { get; set; }
    public List<SubjectProfessorLink>? SubjectLecturers { get; set; }
    public List<SubjectStudentLink>? SubjectStudentLinks { get; set; }
    public List<MajorAnnouncement>? MajorAnnouncements { get; set; }
    public List<Material>? Materials { get; set; }
    public List<Lecture>? Lectures { get; set; }
}

