namespace UniSphere.Api.DTOs.Grades;

public sealed record GradesCollection
{
    public required int NumberOfPassedSubjects { get; init; }
    public required int NumberOfFailedSubjects { get; init; }
    public required double Average { get; init; }
    public required List<GradeDto> Grades { get; init; }
}
