namespace UniSphere.Api.DTOs.Subjects;

public sealed record SuperAdminSubjectsResponseDto
{
    public required List<MajorSubjectsDto> Majors { get; init; }
}

public sealed record ProfessorSubjectsResponseDto
{
    public required string UniversityName { get; init; }
    public required List<FacultySubjectsDto> Faculties { get; init; }
}

public sealed record MajorSubjectsDto
{
    public required string MajorName { get; init; }
    public required List<UnifiedSubjectDto> Subjects { get; init; }
}

public sealed record FacultySubjectsDto
{
    public required string FacultyName { get; init; }
    public required List<MajorSubjectsDto> Majors { get; init; }
} 