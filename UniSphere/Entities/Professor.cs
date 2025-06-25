namespace UniSphere.Api.Entities;

public class Professor
{
    public Guid Id { get; set; }
    public string Gmail { get; set; } = string.Empty;
    public int? OneTimeCode { get; set; }
    public DateTime? OneTimeCodeCreatedDate { get; set; }
    public int? OneTimeCodeExpirationInMinutes { get; set; }
    public MultilingualText Bio { get; set; }=new();
    public MultilingualText FirstName { get; set; }=new();
    public MultilingualText LastName { get; set; }=new();
    public string? Image { get; set; }
    public List<SubjectProfessorLink>? SubjectProfessorLinks { get; set; }
    public List<ProfessorFacultyLink>? ProfessorFacultyLinks { get; set; }
    public string? IdentityId { get; set; }
}
