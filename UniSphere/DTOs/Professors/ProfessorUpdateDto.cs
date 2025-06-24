namespace UniSphere.Api.DTOs.Professors;

public sealed record ProfessorUpdateDto
{
    public string? Gmail { get; set; }
    public string? FirstNameEn { get; set; }
    public string? FirstNameAr { get; set; }
    public string? LastNameEn { get; set; }
    public string? LastNameAr { get; set; }
    public string? BioEn { get; set; }
    public string? BioAr { get; set; }
    public string? Image { get; set; }
} 