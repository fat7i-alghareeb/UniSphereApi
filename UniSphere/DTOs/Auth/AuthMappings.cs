using UniSphere.Api.Controllers;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Auth;

internal static class AuthMappings{
    public static SimpleStudentDto ToSimpleStudentDto(this StudentCredential studentCredential, Languages lang)
    {
        return new SimpleStudentDto
        {
            FullName =  studentCredential.FirstName.GetTranslatedString(lang) + " " + studentCredential.LastName.GetTranslatedString(lang),
            StudentNumber = studentCredential.StudentNumber,
            MajorName = studentCredential.Major.Name.GetTranslatedString(lang),
            MajorId = studentCredential.MajorId,
            StudentId = studentCredential.Id,
            Year = studentCredential.Year
        };
    }
    public static FullInfoStudentDto ToFullInfoStudentDto(this StudentCredential studentCredential,string accessToken,string refreshToken, Languages lang)
    {
        return new FullInfoStudentDto
        {
            FirstName = studentCredential.FirstName.GetTranslatedString(lang),
            LastName = studentCredential.LastName.GetTranslatedString(lang),
            FatherName =studentCredential.FatherName is not null? studentCredential.FatherName!.GetTranslatedString(lang): "" ,
            EnrollmentStatusName = studentCredential.EnrollmentStatus.Name.GetTranslatedString(lang),
            Year = studentCredential.Year,
            StudentNumber = studentCredential.StudentNumber,
            MajorName = studentCredential.Major.Name.GetTranslatedString(lang),
            MajorId = studentCredential.MajorId,
            StudentId = studentCredential.Id,
            StudentImageUrl = studentCredential.Image ?? "",
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            NumberOfMajorYears = studentCredential.Major.NumberOfYears
        };
    }

    public static BaseStudentDto ToBaseStudentDto(this StudentCredential studentCredential , Languages lang)
    {
        return new BaseStudentDto()
        {
            FirstName = studentCredential.FirstName.GetTranslatedString(lang),
            LastName = studentCredential.LastName.GetTranslatedString(lang),
            FatherName =studentCredential.FatherName is not null? studentCredential.FatherName!.GetTranslatedString(lang): "" ,
            EnrollmentStatusName = studentCredential.EnrollmentStatus.Name.GetTranslatedString(lang),
            Year = studentCredential.Year,
            StudentNumber = studentCredential.StudentNumber,
            MajorName = studentCredential.Major.Name.GetTranslatedString(lang),
            MajorId = studentCredential.MajorId,
            StudentId = studentCredential.Id,
            StudentImageUrl = studentCredential.Image ?? "",
            NumberOfMajorYears = studentCredential.Major.NumberOfYears
        };
    }

    // Admin mappings
    public static SimpleAdminDto ToSimpleAdminDto(this Admin admin, Languages lang)
    {
        return new SimpleAdminDto
        {
            FullName = admin.FirstName.GetTranslatedString(lang) + " " + admin.LastName.GetTranslatedString(lang),
            Gmail = admin.Gmail,
            MajorName = admin.Major.Name.GetTranslatedString(lang),
            MajorId = admin.MajorId,
            AdminId = admin.Id
        };
    }

    public static FullInfoAdminDto ToFullInfoAdminDto(this Admin admin, string accessToken, string refreshToken, Languages lang)
    {
        return new FullInfoAdminDto
        {
            FirstName = admin.FirstName.GetTranslatedString(lang),
            LastName = admin.LastName.GetTranslatedString(lang),
            Gmail = admin.Gmail,
            MajorName = admin.Major.Name.GetTranslatedString(lang),
            MajorId = admin.MajorId,
            AdminId = admin.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public static BaseAdminDto ToBaseAdminDto(this Admin admin, Languages lang)
    {
        return new BaseAdminDto()
        {
            FirstName = admin.FirstName.GetTranslatedString(lang),
            LastName = admin.LastName.GetTranslatedString(lang),
            Gmail = admin.Gmail,
            MajorName = admin.Major.Name.GetTranslatedString(lang),
            MajorId = admin.MajorId,
            AdminId = admin.Id
        };
    }
}
