using UniSphere.Api.Controllers;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Grades;

internal static class GradeMappings
{
    public static GradeDto ToDto(this SubjectStudentLink subjectStudentLink , Languages lang)
    {
        return new GradeDto
        {
        SubjectYear = subjectStudentLink.Subject.Year,
        SubjectSemester = subjectStudentLink.Subject.Semester,
        MidTermGrade = subjectStudentLink.MidtermGrade ?? 0,        
        FinalGrade = subjectStudentLink.FinalGrade?? 0,
        IsPassed = subjectStudentLink.IsPassed,
        SubjectName = subjectStudentLink.Subject.Name.GetTranslatedString(lang),
        };
    }
    
    
}
