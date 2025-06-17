using System;
using System.Security.Claims;

namespace UniSphere.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetStudentId(this ClaimsPrincipal principal)
    {
        string? studentIdString = principal.FindFirstValue("studentId");
        
        // Return null if parsing fails
        if (string.IsNullOrWhiteSpace(studentIdString) || !Guid.TryParse(studentIdString, out Guid studentId))
        {
            return null;
        }

        return studentId;
    }
}
