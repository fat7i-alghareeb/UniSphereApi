namespace UniSphere.Api.DTOs.Subjects;

public sealed record UnifiedSubjectDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required Guid MajorId { get; init; }
    public Guid? LabId { get; init; }
    public required int Year { get; init; }
    public required int Semester { get; init; }
    public required int MidtermGrade { get; init; }
    public required int FinalGrade { get; init; }
    public required bool IsLabRequired { get; init; }
    public required bool IsMultipleChoice { get; init; }
    public required bool IsOpenBook { get; init; }
    public string? Image { get; init; }
    public required List<MaterialInfo> Materials { get; init; }
} 