using UniSphere.Api.Controllers;

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

public static class EligibleStudentMappings
{
    public static EligibleStudentDto ToEligibleStudentDto(this UniSphere.Api.Entities.StudentCredential student , Languages lang)
    {
        return new EligibleStudentDto
        {
            Id = student.Id,
            StudentNumber = student.StudentNumber,
            FullName = $"{student.FirstName.GetTranslatedString(lang)} {student.LastName.GetTranslatedString(lang)}".Trim()
        };
    }
} 