using System.ComponentModel.DataAnnotations.Schema;

namespace UniSphere.Api.Entities;

public class InstructorLab
{
    public Guid InstructorId { get; set; }
    public Guid LabId { get; set; }
    public Instructor Instructor { get; set; } = null!;
    public Lab Lab { get; set; } = null!;
} 