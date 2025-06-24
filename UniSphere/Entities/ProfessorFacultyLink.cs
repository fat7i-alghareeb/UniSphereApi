namespace UniSphere.Api.Entities;

public class ProfessorFacultyLink
{
    public Guid ProfessorId { get; set; }
    public Guid FacultyId { get; set; }
    public Professor Professor { get; set; } = null!;
    public Faculty Faculty { get; set; } = null!;
} 