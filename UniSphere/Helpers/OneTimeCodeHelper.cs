using System;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Helpers;

public static class OneTimeCodeHelper
{
    public static void AssignOneTimeCode(StudentCredential student, int code, int expirationInMinutes)
    {
        student.OneTimeCode = code;
        student.OneTimeCodeCreatedDate = DateTime.UtcNow;
        student.OneTimeCodeExpirationInMinutes = expirationInMinutes;
    }

    public static void AssignOneTimeCode(Admin admin, int code, int expirationInMinutes)
    {
        admin.OneTimeCode = code;
        admin.OneTimeCodeCreatedDate = DateTime.UtcNow;
        admin.OneTimeCodeExpirationInMinutes = expirationInMinutes;
    }

    public static void AssignOneTimeCode(SuperAdmin superAdmin, int code, int expirationInMinutes)
    {
        superAdmin.OneTimeCode = code;
        superAdmin.OneTimeCodeCreatedDate = DateTime.UtcNow;
        superAdmin.OneTimeCodeExpirationInMinutes = expirationInMinutes;
    }

    public static void AssignOneTimeCode(Professor professor, int code, int expirationInMinutes)
    {
        professor.OneTimeCode = code;
        professor.OneTimeCodeCreatedDate = DateTime.UtcNow;
        professor.OneTimeCodeExpirationInMinutes = expirationInMinutes;
    }
} 