namespace UniSphere.Api.DTOs.Info;

public sealed record EligibleStudentDto
{
    public required Guid Id { get; init; }
    public required string StudentNumber { get; init; }
    public required string FullName { get; init; }
}

public sealed record EligibleStudentsCollectionDto
{
    public required List<EligibleStudentDto> Students { get; init; }
} 