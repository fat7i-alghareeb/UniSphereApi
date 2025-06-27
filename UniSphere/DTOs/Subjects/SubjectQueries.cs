using System.Linq.Expressions;
using UniSphere.Api.Controllers;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Subjects;

internal static class SubjectQueries
{

    public static Expression<Func<Subject, SubjectDto>> ProjectToDto(  Guid studentId ,Languages lang )
    {
        return subject => subject.ToDto(studentId,lang);
    }
    
    public static Expression<Func<Subject, UnifiedSubjectDto>> ProjectToUnifiedDto(Languages lang)
    {
        return subject => subject.ToUnifiedDto(lang);
    }
}
