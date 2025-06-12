using System.ComponentModel.DataAnnotations.Schema;

namespace UniSphere.Api.Entities;

public class SubjectProfessorLink
{
    public Guid SubjectId { get; set; }
    public Guid ProfessorId { get; set; }
    public Subject? Subject { get; set; }
    public Professor? Professor { get; set; }
} 