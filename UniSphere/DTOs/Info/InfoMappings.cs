using UniSphere.Api.Controllers;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Info;

internal static class InfoMappings
{
    public static FacultyNameDto ToFacultyNameDto(this Faculty faculty, Languages lang)
    {
        return new FacultyNameDto
        {
            Id = faculty.Id,
            Name = faculty.Name.GetTranslatedString(lang)
        };
    }

    public static MajorNameDto ToFacultyNameDto(this Major major, Languages lang)
    {
        return new MajorNameDto()
        {
            Id = major.Id,
            Name = major.Name.GetTranslatedString(lang)
        };
    }
}
