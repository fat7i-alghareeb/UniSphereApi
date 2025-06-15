using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Auth;

internal static class AuthMappings{
    public static SimpleStudentDto ToSimpleStudentDto(this StudentCredential studentCredential)
    {
        return new SimpleStudentDto
        {
            FullName = studentCredential.FirstName.Ar + " " + studentCredential.LastName.Ar,
            StudentNumber = studentCredential.StudentNumber,
            MajorName = studentCredential.Major.Name.Ar,
            MajorId = studentCredential.MajorId,
            StudentId = studentCredential.Id,
            Year = studentCredential.Year
        };
    }
    public static FullInfoStudentDto ToFullInfoStudentDto(this StudentCredential studentCredential,string accessToken,string refreshToken)
    {
        return new FullInfoStudentDto
        {
            FirstName = studentCredential.FirstName.Ar,
            LastName = studentCredential.LastName.Ar,
            FatherName = studentCredential.FatherName?.Ar ?? "",
            EnrollmentStatusName = studentCredential.EnrollmentStatus.Name.Ar,
            Year = studentCredential.Year,
            StudentNumber = studentCredential.StudentNumber,
            MajorName = studentCredential.Major.Name.Ar,
            MajorId = studentCredential.MajorId,
            StudentId = studentCredential.Id,
            StudentImageUrl = studentCredential.Image ?? "",
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public static BaseStudentDto ToBaseStudentDto(this StudentCredential studentCredential)
    {
        return new BaseStudentDto()
        {
            FirstName = studentCredential.FirstName.Ar,
            LastName = studentCredential.LastName.Ar,
            FatherName = studentCredential.FatherName?.Ar ?? "",
            EnrollmentStatusName = studentCredential.EnrollmentStatus.Name.Ar,
            Year = studentCredential.Year,
            StudentNumber = studentCredential.StudentNumber,
            MajorName = studentCredential.Major.Name.Ar,
            MajorId = studentCredential.MajorId,
            StudentId = studentCredential.Id,
            StudentImageUrl = studentCredential.Image ?? "",
        };
    }
}
