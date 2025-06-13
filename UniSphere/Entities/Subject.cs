using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;

namespace UniSphere.Api.Entities;

public class Subject
{
    public Guid Id { get; set; }
    public MultilingualText Name { get; set; } = new();
    public Guid MajorId { get; set; }
    public Guid? LabId { get; set; }
    public MultilingualText? Description { get; set; }
    public int Year { get; set; }
    public int Semester { get; set; }
    public int MidtermGrade { get; set; } = 30;
    public int FinalGrade { get; set; } = 70;
    public bool IsLabRequired { get; set; }
    public bool IsMultipleChoice { get; set; }
    public bool IsOpenBook { get; set; }
    public string? Image { get; set; }
        
    public Major Major { get; set; } = null!;
    public Lab? Lab { get; set; }
    public List<SubjectProfessorLink>? SubjectLecturers { get; set; }
    public List<SubjectStudentLink>? SubjectStudentLinks { get; set; }
}

