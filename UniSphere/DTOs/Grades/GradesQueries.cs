using System.Linq.Expressions;
using UniSphere.Api.Controllers;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Grades;

internal static class GradesQueries
{
    public static Expression<Func<SubjectStudentLink,  GradeDto>> ProjectToDto(Languages lang)
    {
        return subjectStudentLink => subjectStudentLink.ToDto(lang);
    } 
}
