using System.ComponentModel.DataAnnotations;

namespace UniSphere.Api.Entities;

public class Professor
{
    public Guid Id { get; set; }
    public MultilingualText FirstName { get; set; }=new();
    public MultilingualText LastName { get; set; }=new();
    public string? Image { get; set; }
    public List<SubjectProfessorLink>? SubjectProfessorLinks { get; set; }
}
