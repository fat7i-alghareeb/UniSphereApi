using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Subjects;

internal static class SubjectMappings
{
    public static SubjectDto ToDto(this Subject subject , Guid studentId )
    {

        var professor =subject.SubjectLecturers?.FirstOrDefault(spl=>spl.SubjectId == subject.Id )?.Professor;
        var midTermGrade = subject.SubjectStudentLinks?.FirstOrDefault(ss => ss.SubjectId == subject.Id && ss.StudentId == studentId)?.MidtermGrade ;
        var finalGrade = subject.SubjectStudentLinks?.FirstOrDefault(ss => ss.SubjectId == subject.Id && ss.StudentId == studentId)?.FinalGrade ;
        return new SubjectDto
        {
            Id = subject.Id,
            Year = subject.Year,
            Semester = subject.Semester,
            Name = subject.Name.Ar, 
            ProfessorName = professor?.FirstName.Ar + " " + professor?.LastName.Ar ,
            ImageUrl = subject.Image,
            MidTermGrade = midTermGrade,
            FinalGrade = finalGrade,
            IsPassed = subject.SubjectStudentLinks?.Any(ss => ss.StudentId == studentId && ss.IsPassed) ?? false,
            CanEnroll = subject.SubjectStudentLinks?.Any(ss => ss.StudentId == studentId && ss is { IsCurrentlyEnrolled: true, IsPassed: false } ) ?? false,
            IsMultipleChoice = subject.IsMultipleChoice ,
            DoesHaveALab = subject.IsLabRequired ,
            MajorId = subject.MajorId
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
        return subject;

    }
}
