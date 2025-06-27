using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Auth;

internal static class AuthMappings{
    public static SimpleStudentDto ToSimpleStudentDto(this StudentCredential studentCredential)
    {
        return new SimpleStudentDto
        {
            FullName = new MultilingualNameDto 
            { 
                En = studentCredential.FirstName.En ?? "", 
                Ar = studentCredential.FirstName.Ar ?? "" 
            },
            StudentNumber = studentCredential.StudentNumber,
            MajorName = new MultilingualNameDto 
            { 
                En = studentCredential.Major.Name.En ?? "", 
                Ar = studentCredential.Major.Name.Ar ?? "" 
            },
            MajorId = studentCredential.MajorId,
            StudentId = studentCredential.Id,
            Year = studentCredential.Year
        };
    }
    public static FullInfoStudentDto ToFullInfoStudentDto(this StudentCredential studentCredential, string accessToken, string refreshToken, string? role = null)
    {
        return new FullInfoStudentDto
        {
            FirstName = new MultilingualNameDto 
            { 
                En = studentCredential.FirstName.En ?? "", 
                Ar = studentCredential.FirstName.Ar ?? "" 
            },
            LastName = new MultilingualNameDto 
            { 
                En = studentCredential.LastName.En ?? "", 
                Ar = studentCredential.LastName.Ar ?? "" 
            },
            FatherName = new MultilingualNameDto 
            { 
                En = studentCredential.FatherName?.En ?? "", 
                Ar = studentCredential.FatherName?.Ar ?? "" 
            },
            EnrollmentStatusName = new MultilingualNameDto 
            { 
                En = studentCredential.EnrollmentStatus.Name.En ?? "", 
                Ar = studentCredential.EnrollmentStatus.Name.Ar ?? "" 
            },
            Year = studentCredential.Year,
            StudentNumber = studentCredential.StudentNumber,
            MajorName = new MultilingualNameDto 
            { 
                En = studentCredential.Major.Name.En ?? "", 
                Ar = studentCredential.Major.Name.Ar ?? "" 
            },
            MajorId = studentCredential.MajorId,
            StudentId = studentCredential.Id,
            StudentImageUrl = studentCredential.Image ?? "",
            Role = role ?? "Student",
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            NumberOfMajorYears = studentCredential.Major.NumberOfYears
        };
    }

    public static BaseStudentDto ToBaseStudentDto(this StudentCredential studentCredential, string? role = null)
    {
        return new BaseStudentDto()
        {
            FirstName = new MultilingualNameDto 
            { 
                En = studentCredential.FirstName.En ?? "", 
                Ar = studentCredential.FirstName.Ar ?? "" 
            },
            LastName = new MultilingualNameDto 
            { 
                En = studentCredential.LastName.En ?? "", 
                Ar = studentCredential.LastName.Ar ?? "" 
            },
            FatherName = new MultilingualNameDto 
            { 
                En = studentCredential.FatherName?.En ?? "", 
                Ar = studentCredential.FatherName?.Ar ?? "" 
            },
            EnrollmentStatusName = new MultilingualNameDto 
            { 
                En = studentCredential.EnrollmentStatus.Name.En ?? "", 
                Ar = studentCredential.EnrollmentStatus.Name.Ar ?? "" 
            },
            Year = studentCredential.Year,
            StudentNumber = studentCredential.StudentNumber,
            MajorName = new MultilingualNameDto 
            { 
                En = studentCredential.Major.Name.En ?? "", 
                Ar = studentCredential.Major.Name.Ar ?? "" 
            },
            MajorId = studentCredential.MajorId,
            StudentId = studentCredential.Id,
            StudentImageUrl = studentCredential.Image ?? "",
            Role = role ?? "Student",
            NumberOfMajorYears = studentCredential.Major.NumberOfYears
        };
    }

    // Admin mappings
    public static SimpleAdminDto ToSimpleAdminDto(this Admin admin)
    {
        return new SimpleAdminDto
        {
            FullName = new MultilingualNameDto 
            { 
                En = admin.FirstName.En ?? "", 
                Ar = admin.FirstName.Ar ?? "" 
            },
            Gmail = admin.Gmail,
            MajorName = new MultilingualNameDto 
            { 
                En = admin.Major.Name.En ?? "", 
                Ar = admin.Major.Name.Ar ?? "" 
            },
            MajorId = admin.MajorId,
            AdminId = admin.Id
        };
    }

    public static FullInfoAdminDto ToFullInfoAdminDto(this Admin admin, string accessToken, string refreshToken, string? role = null)
    {
        return new FullInfoAdminDto
        {
            FirstName = new MultilingualNameDto 
            { 
                En = admin.FirstName.En ?? "", 
                Ar = admin.FirstName.Ar ?? "" 
            },
            LastName = new MultilingualNameDto 
            { 
                En = admin.LastName.En ?? "", 
                Ar = admin.LastName.Ar ?? "" 
            },
            Gmail = admin.Gmail,
            MajorName = new MultilingualNameDto 
            { 
                En = admin.Major.Name.En ?? "", 
                Ar = admin.Major.Name.Ar ?? "" 
            },
            MajorId = admin.MajorId,
            AdminId = admin.Id,
            Role = role ?? "Admin",
            Image = admin.Image,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public static BaseAdminDto ToBaseAdminDto(this Admin admin, string? role = null)
    {
        return new BaseAdminDto()
        {
            FirstName = new MultilingualNameDto 
            { 
                En = admin.FirstName.En ?? "", 
                Ar = admin.FirstName.Ar ?? "" 
            },
            LastName = new MultilingualNameDto 
            { 
                En = admin.LastName.En ?? "", 
                Ar = admin.LastName.Ar ?? "" 
            },
            Gmail = admin.Gmail,
            MajorName = new MultilingualNameDto 
            { 
                En = admin.Major.Name.En ?? "", 
                Ar = admin.Major.Name.Ar ?? "" 
            },
            MajorId = admin.MajorId,
            AdminId = admin.Id,
            Role = role ?? "Admin",
            Image = admin.Image
        };
    }

    // SuperAdmin mappings
    public static SimpleSuperAdminDto ToSimpleSuperAdminDto(this SuperAdmin superAdmin)
    {
        return new SimpleSuperAdminDto
        {
            FullName = new MultilingualNameDto 
            { 
                En = superAdmin.FirstName.En ?? "", 
                Ar = superAdmin.FirstName.Ar ?? "" 
            },
            Gmail = superAdmin.Gmail,
            FacultyName = new MultilingualNameDto 
            { 
                En = superAdmin.Faculty.Name.En ?? "", 
                Ar = superAdmin.Faculty.Name.Ar ?? "" 
            },
            FacultyId = superAdmin.FacultyId,
            SuperAdminId = superAdmin.Id
        };
    }

    public static FullInfoSuperAdminDto ToFullInfoSuperAdminDto(this SuperAdmin superAdmin, string accessToken, string refreshToken, string? role = null)
    {
        return new FullInfoSuperAdminDto
        {
            FirstName = new MultilingualNameDto 
            { 
                En = superAdmin.FirstName.En ?? "", 
                Ar = superAdmin.FirstName.Ar ?? "" 
            },
            LastName = new MultilingualNameDto 
            { 
                En = superAdmin.LastName.En ?? "", 
                Ar = superAdmin.LastName.Ar ?? "" 
            },
            Gmail = superAdmin.Gmail,
            FacultyName = new MultilingualNameDto 
            { 
                En = superAdmin.Faculty.Name.En ?? "", 
                Ar = superAdmin.Faculty.Name.Ar ?? "" 
            },
            FacultyId = superAdmin.FacultyId,
            SuperAdminId = superAdmin.Id,
            Role = role ?? "SuperAdmin",
            Image = superAdmin.Image,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public static BaseSuperAdminDto ToBaseSuperAdminDto(this SuperAdmin superAdmin, string? role = null)
    {
        return new BaseSuperAdminDto()
        {
            FirstName = new MultilingualNameDto 
            { 
                En = superAdmin.FirstName.En ?? "", 
                Ar = superAdmin.FirstName.Ar ?? "" 
            },
            LastName = new MultilingualNameDto 
            { 
                En = superAdmin.LastName.En ?? "", 
                Ar = superAdmin.LastName.Ar ?? "" 
            },
            Gmail = superAdmin.Gmail,
            FacultyName = new MultilingualNameDto 
            { 
                En = superAdmin.Faculty.Name.En ?? "", 
                Ar = superAdmin.Faculty.Name.Ar ?? "" 
            },
            FacultyId = superAdmin.FacultyId,
            SuperAdminId = superAdmin.Id,
            Role = role ?? "SuperAdmin",
            Image = superAdmin.Image
        };
    }

    // Professor mappings
    public static SimpleProfessorDto ToSimpleProfessorDto(this Professor professor)
    {
        return new SimpleProfessorDto
        {
            FullName = new MultilingualNameDto 
            { 
                En = professor.FirstName.En ?? "", 
                Ar = professor.FirstName.Ar ?? "" 
            },
            Gmail = professor.Gmail,
            ProfessorId = professor.Id
        };
    }

    public static FullInfoProfessorDto ToFullInfoProfessorDto(this Professor professor, string accessToken, string refreshToken, string? role = null)
    {
        return new FullInfoProfessorDto
        {
            FirstName = new MultilingualNameDto 
            { 
                En = professor.FirstName.En ?? "", 
                Ar = professor.FirstName.Ar ?? "" 
            },
            LastName = new MultilingualNameDto 
            { 
                En = professor.LastName.En ?? "", 
                Ar = professor.LastName.Ar ?? "" 
            },
            Gmail = professor.Gmail,
            Bio = new MultilingualTextDto 
            { 
                En = professor.Bio.En ?? "", 
                Ar = professor.Bio.Ar ?? "" 
            },
            Image = professor.Image ?? "",
            ProfessorId = professor.Id,
            Role = role ?? "Professor",
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public static BaseProfessorDto ToBaseProfessorDto(this Professor professor, string? role = null)
    {
        return new BaseProfessorDto()
        {
            FirstName = new MultilingualNameDto 
            { 
                En = professor.FirstName.En ?? "", 
                Ar = professor.FirstName.Ar ?? "" 
            },
            LastName = new MultilingualNameDto 
            { 
                En = professor.LastName.En ?? "", 
                Ar = professor.LastName.Ar ?? "" 
            },
            Gmail = professor.Gmail,
            Bio = new MultilingualTextDto 
            { 
                En = professor.Bio.En ?? "", 
                Ar = professor.Bio.Ar ?? "" 
            },
            Image = professor.Image ?? "",
            ProfessorId = professor.Id,
            Role = role ?? "Professor"
        };
    }

    // SystemController mappings
    public static SimpleSystemControllerDto ToSimpleSystemControllerDto(this SystemController systemController)
    {
        return new SimpleSystemControllerDto
        {
            FullName = new MultilingualNameDto 
            { 
                En = systemController.UserName, 
                Ar = systemController.UserName 
            },
            Gmail = systemController.Gmail,
            UserName = systemController.UserName,
            SystemControllerId = systemController.Id
        };
    }

    public static FullInfoSystemControllerDto ToFullInfoSystemControllerDto(this SystemController systemController, string accessToken, string refreshToken, string? role = null)
    {
        return new FullInfoSystemControllerDto
        {
            Gmail = systemController.Gmail,
            UserName = systemController.UserName,
            SystemControllerId = systemController.Id,
            Role = role ?? "SystemController",
            Image = systemController.Image,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public static BaseSystemControllerDto ToBaseSystemControllerDto(this SystemController systemController, string? role = null)
    {
        return new BaseSystemControllerDto()
        {
            Gmail = systemController.Gmail,
            UserName = systemController.UserName,
            SystemControllerId = systemController.Id,
            Role = role ?? "SystemController",
            Image = systemController.Image
        };
    }
}
