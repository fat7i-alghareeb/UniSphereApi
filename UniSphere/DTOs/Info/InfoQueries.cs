using System.Linq.Expressions;
using UniSphere.Api.Controllers;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Info;

internal static class InfoQueries
{
    public static Expression<Func<Faculty, FacultyNameDto>> ProjectToFacultyNameDto(Languages lang) =>
        faculty => faculty.ToFacultyNameDto(lang);
    
    public static Expression<Func<Major, MajorNameDto>> ProjectToMajorNameDto(Languages lang) =>
        major => major.ToFacultyNameDto(lang);
}
