using System.Security.Claims;

namespace UniSphere.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetStudentId(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue("studentId");
    }
}
