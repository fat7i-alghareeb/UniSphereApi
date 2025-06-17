using System.Linq.Expressions;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Subjects;

internal static class SubjectQueries
{

    public static Expression<Func<Subject, SubjectDto>> ProjectToDto(  Guid studentId )
    {
        return subject => subject.ToDto(studentId);
    }
}
