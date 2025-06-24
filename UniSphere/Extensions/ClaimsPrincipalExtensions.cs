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
    public static Guid? GetAdminId(this ClaimsPrincipal principal)
    {
        string? adminIdString = principal.FindFirstValue("adminId");
        if (string.IsNullOrWhiteSpace(adminIdString) || !Guid.TryParse(adminIdString, out Guid adminId))
        {
            return null;
        }
        return adminId;
    }
    public static Guid? GetSuperAdminId(this ClaimsPrincipal principal)
    {
        string? superAdminIdString = principal.FindFirstValue("superAdminId");
        if (string.IsNullOrWhiteSpace(superAdminIdString) || !Guid.TryParse(superAdminIdString, out Guid superAdminId))
        {
            return null;
        }
        return superAdminId;
    }
    public static Guid? GetProfessorId(this ClaimsPrincipal principal)
    {
        string? professorIdString = principal.FindFirstValue("professorId");
        if (string.IsNullOrWhiteSpace(professorIdString) || !Guid.TryParse(professorIdString, out Guid professorId))
        {
            return null;
        }
        return professorId;
    }
    public static Guid? GetSystemControllerId(this ClaimsPrincipal principal)
    {
        string? systemControllerIdString = principal.FindFirstValue("systemControllerId");
        if (string.IsNullOrWhiteSpace(systemControllerIdString) || !Guid.TryParse(systemControllerIdString, out Guid systemControllerId))
        {
            return null;
        }
        return systemControllerId;
    }
}
