using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Subjects;

internal static class SubjectMappings
{
    public static SubjectDto ToDto(this Subject subject)
    {

        return new SubjectDto
        {

            Id = subject.Id,
            MajorId = subject.MajorId,
            Year = subject.Year,
            Semester = subject.Semester,
            MajoreName = subject.Major.Name.Ar

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
            FinalGrade = 70,
            Year =1,
            Semester = 1,
        };
        return subject;
    }

    public static Subject UpdateFromDto(this Subject subject, SubjectDto subjectDto)
    {
        subject.Id = subjectDto.Id;
        subject.MajorId = subjectDto.MajorId;
        return subject;

    }
}
