using System.ComponentModel.DataAnnotations.Schema;

namespace UniSphere.Api.Entities;

public class InstructorLabLink
{
    public Guid InstructorId { get; set; }
    public Guid LabId { get; set; }
    public Instructor Instructor { get; set; } = null!;
    public Lab Lab { get; set; } = null!;
} 