using UniSphere.Api.Controllers;
using UniSphere.Api.Entities;
using UniSphere.Api.Services;

namespace UniSphere.Api.DTOs.Subjects;

internal static class SubjectMappings
{
    public static SubjectDto ToDto(this Subject subject , Guid studentId,Languages lang )
    {
        var professor =subject.SubjectLecturers?.FirstOrDefault(spl=>spl.SubjectId == subject.Id )?.Professor;
        var midTermGrade = subject.SubjectStudentLinks?.FirstOrDefault(ss => ss.SubjectId == subject.Id && ss.StudentId == studentId)?.MidtermGrade ;
        var finalGrade = subject.SubjectStudentLinks?.FirstOrDefault(ss => ss.SubjectId == subject.Id && ss.StudentId == studentId)?.FinalGrade ;
        
        return new SubjectDto
        {
            Id = subject.Id,
            Year = subject.Year,
            Semester = subject.Semester,
            Name = subject.Name.GetTranslatedString(lang), 
            ProfessorName = professor is not null ? professor.FirstName.GetTranslatedString(lang) + " " + professor.LastName.GetTranslatedString(lang) : null,
            ImageUrl = subject.Image,
            MidTermGrade = midTermGrade,
            FinalGrade = finalGrade,
            IsPassed = subject.SubjectStudentLinks?.Any(ss => ss.StudentId == studentId && ss.IsPassed) ?? false,
            CanEnroll = subject.SubjectStudentLinks?.Any(ss => ss.StudentId == studentId && ss is { IsCurrentlyEnrolled: true, IsPassed: false } ) ?? false,
            IsMultipleChoice = subject.IsMultipleChoice ,
            DoesHaveALab = subject.IsLabRequired ,
            MajorId = subject.MajorId,
            Materials = subject.Materials?.Select(m => new MaterialInfo
            {
                Url = m.Url,
                Type = m.Type,
                PassGrade = subject.PassGrade
            }).ToList() ?? new List<MaterialInfo>(),
            PassGrade = subject.PassGrade
        };
    }
    
    public static UnifiedSubjectDto ToUnifiedDto(this Subject subject, Languages lang)
    {
        return new UnifiedSubjectDto
        {
            Id = subject.Id,
            Name = subject.Name.GetTranslatedString(lang),
            Description = subject.Description.GetTranslatedString(lang),
            MajorId = subject.MajorId,
            LabId = subject.LabId,
            Year = subject.Year,
            Semester = subject.Semester,
            MidtermGrade = subject.MidtermGrade,
            FinalGrade = subject.FinalGrade,
            IsLabRequired = subject.IsLabRequired,
            IsMultipleChoice = subject.IsMultipleChoice,
            IsOpenBook = subject.IsOpenBook,
            Image = subject.Image,
            Materials = subject.Materials?.Select(m => new MaterialInfo
            {
                Url = m.Url,
                Type = m.Type,
                PassGrade = subject.PassGrade
            }).ToList() ?? new List<MaterialInfo>(),
            PassGrade = subject.PassGrade
        };
    }
    
    public static Subject ToEntity(this CreateSubjectDto dto)
    {
        Subject subject = new()
        {
            Id = Guid.NewGuid(),
            Name = new MultilingualText { En = dto.NameEn, Ar = dto.NameAr },
            Description = new MultilingualText { En = dto.DescriptionEn, Ar = dto.DescriptionAr },
            MajorId = dto.MajorId,
            LabId = dto.LabId,
            Year = dto.Year,
            Semester = dto.Semester,
            MidtermGrade = dto.MidtermGrade,
            FinalGrade = dto.FinalGrade,
            IsLabRequired = dto.IsLabRequired,
            IsMultipleChoice = dto.IsMultipleChoice,
            IsOpenBook = dto.IsOpenBook,
            Image = dto.Image,
            PassGrade = dto.PassGrade
        };
        return subject;
    }

    public static Subject UpdateFromDto(this Subject subject, SubjectDto subjectDto)
    {
        subject.Id = subjectDto.Id;
        subject.PassGrade = subjectDto.PassGrade;
        return subject;
    }

    public static SubjectNameIdDto ToSubjectNameIdDto(this Subject subject, Languages lang)
    {
        return new SubjectNameIdDto
        {
            Id = subject.Id,
            Name = subject.Name.GetTranslatedString(lang)
        };
    }
}
