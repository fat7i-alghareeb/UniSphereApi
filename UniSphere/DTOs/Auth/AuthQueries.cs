using System.Linq.Expressions;
using UniSphere.Api.Controllers;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Auth;

internal static class AuthQueries
{
    public static Expression<Func<StudentCredential, FullInfoStudentDto>> ProjectToDto(string accessToken, string refreshToken ,Languages lang)
    {
        return credential => credential.ToFullInfoStudentDto(accessToken, refreshToken, lang);
    }

}
