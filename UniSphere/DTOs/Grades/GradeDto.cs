namespace UniSphere.Api.DTOs.Grades;

public sealed record GradeDto
{
    public required int SubjectYear { get; init; }
    public required int SubjectSemester { get; init; }
    public required double  MidTermGrade { get; init; }
    public required double FinalGrade { get; init; }
    public required string SubjectName { get; init; }
    public required bool IsPassed { get; init; }
    

}
