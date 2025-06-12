using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Subjects;

internal static class SubjectMappings
{
    public static SubjectDto ToDto(this Subject subject)
    {

        return new SubjectDto
        {

            Id = subject.Id

        };
    }
    public static Subject ToEntity(this CreateSubjectDto dto)
    {

        Subject subject = new()
        {
            Id = Guid.NewGuid(),
            Name = new MultilingualText { Ar = dto.Name, En = dto.Name },
            Description = new MultilingualText { Ar = "مقدمة في تطوير تطبيقات الويب", En = "Introduction to Web Application Development" },
            MajorId = dto.MajorId,
            IsLabRequired = true,
            IsMultipleChoice = false,
            IsOpenBook = false,
            MidtermGrade = 30,
            FinalGrade = 70
        };
        return subject;
    }
}
