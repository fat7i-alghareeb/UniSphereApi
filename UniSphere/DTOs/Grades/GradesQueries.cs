using System.Linq.Expressions;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Grades;

internal static class GradesQueries
{
    public static Expression<Func<SubjectStudentLink,  GradeDto>> ProjectToDto()
    {
        return subjectStudentLink => subjectStudentLink.ToDto();
    } 
}
