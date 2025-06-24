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

    // SuperAdmin mappings
    public static SimpleSuperAdminDto ToSimpleSuperAdminDto(this SuperAdmin superAdmin, Languages lang)
    {
        return new SimpleSuperAdminDto
        {
            FullName = superAdmin.FirstName.GetTranslatedString(lang) + " " + superAdmin.LastName.GetTranslatedString(lang),
            Gmail = superAdmin.Gmail,
            FacultyName = superAdmin.Faculty.Name.GetTranslatedString(lang),
            FacultyId = superAdmin.FacultyId,
            SuperAdminId = superAdmin.Id
        };
    }

    public static FullInfoSuperAdminDto ToFullInfoSuperAdminDto(this SuperAdmin superAdmin, string accessToken, string refreshToken, Languages lang)
    {
        return new FullInfoSuperAdminDto
        {
            FirstName = superAdmin.FirstName.GetTranslatedString(lang),
            LastName = superAdmin.LastName.GetTranslatedString(lang),
            Gmail = superAdmin.Gmail,
            FacultyName = superAdmin.Faculty.Name.GetTranslatedString(lang),
            FacultyId = superAdmin.FacultyId,
            SuperAdminId = superAdmin.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public static BaseSuperAdminDto ToBaseSuperAdminDto(this SuperAdmin superAdmin, Languages lang)
    {
        return new BaseSuperAdminDto()
        {
            FirstName = superAdmin.FirstName.GetTranslatedString(lang),
            LastName = superAdmin.LastName.GetTranslatedString(lang),
            Gmail = superAdmin.Gmail,
            FacultyName = superAdmin.Faculty.Name.GetTranslatedString(lang),
            FacultyId = superAdmin.FacultyId,
            SuperAdminId = superAdmin.Id
        };
    }

    // Professor mappings
    public static SimpleProfessorDto ToSimpleProfessorDto(this Professor professor, Languages lang)
    {
        return new SimpleProfessorDto
        {
            FullName = professor.FirstName.GetTranslatedString(lang) + " " + professor.LastName.GetTranslatedString(lang),
            Gmail = professor.Gmail,
            ProfessorId = professor.Id
        };
    }

    public static FullInfoProfessorDto ToFullInfoProfessorDto(this Professor professor, string accessToken, string refreshToken, Languages lang)
    {
        return new FullInfoProfessorDto
        {
            FirstName = professor.FirstName.GetTranslatedString(lang),
            LastName = professor.LastName.GetTranslatedString(lang),
            Gmail = professor.Gmail,
            Bio = professor.Bio.GetTranslatedString(lang),
            Image = professor.Image ?? "",
            ProfessorId = professor.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public static BaseProfessorDto ToBaseProfessorDto(this Professor professor, Languages lang)
    {
        return new BaseProfessorDto()
        {
            FirstName = professor.FirstName.GetTranslatedString(lang),
            LastName = professor.LastName.GetTranslatedString(lang),
            Gmail = professor.Gmail,
            Bio = professor.Bio.GetTranslatedString(lang),
            Image = professor.Image ?? "",
            ProfessorId = professor.Id
        };
    }

    // SystemController mappings
    public static SimpleSystemControllerDto ToSimpleSystemControllerDto(this SystemController systemController)
    {
        return new SimpleSystemControllerDto
        {
            FullName = systemController.UserName,
            Gmail = systemController.Gmail,
            UserName = systemController.UserName,
            SystemControllerId = systemController.Id
        };
    }

    public static FullInfoSystemControllerDto ToFullInfoSystemControllerDto(this SystemController systemController, string accessToken, string refreshToken)
    {
        return new FullInfoSystemControllerDto
        {
            Gmail = systemController.Gmail,
            UserName = systemController.UserName,
            SystemControllerId = systemController.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public static BaseSystemControllerDto ToBaseSystemControllerDto(this SystemController systemController)
    {
        return new BaseSystemControllerDto()
        {
            Gmail = systemController.Gmail,
            UserName = systemController.UserName,
            SystemControllerId = systemController.Id
        };
    }
}
