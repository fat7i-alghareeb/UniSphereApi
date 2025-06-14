using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class StudentCredentialSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.StudentCredentials.AnyAsync())
        {
            List<Faculty> faculties = await Context.Faculties.ToListAsync();
            List<Major> majors = await Context.Majors.ToListAsync();
            List<EnrollmentStatus> enrollmentStatuses = await Context.EnrollmentStatuses.ToListAsync();
            if (faculties.Count == 0 || majors.Count == 0 || enrollmentStatuses.Count == 0)
            {
                return;
            }
            var studentCredentials = new List<StudentCredential>
            {
                // Informatics Engineering Students
                new()
                {
                    Id = Guid.NewGuid(),
                    StudentNumber = "2024" + Random.Shared.Next(100000, 999999).ToString(System.Globalization.CultureInfo.InvariantCulture),
                    OneTimeCode = 1234,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    FirstName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    LastName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    FatherName = new MultilingualText { Ar = "علي", En = "Ali" },
                    Year = 2,
                    MajorId = majors[0].Id,
                    EnrollmentStatusId = enrollmentStatuses[0].Id,
                    Image = "https://via.placeholder.com/150"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    StudentNumber = "2024" + Random.Shared.Next(100000, 999999).ToString(System.Globalization.CultureInfo.InvariantCulture),
                    OneTimeCode = 1234,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    FirstName = new MultilingualText { Ar = "فاطمة", En = "Fatima" },
                    LastName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    FatherName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    Year = 2,
                    MajorId = majors[0].Id,
                    EnrollmentStatusId = enrollmentStatuses[0].Id,
                    Image = "https://via.placeholder.com/150"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    StudentNumber = "2024" + Random.Shared.Next(100000, 999999).ToString(System.Globalization.CultureInfo.InvariantCulture),
                    OneTimeCode = 1234,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    FirstName = new MultilingualText { Ar = "علي", En = "Ali" },
                    LastName = new MultilingualText { Ar = "حسين", En = "Hussein" },
                    FatherName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    Year = 3,
                    MajorId = majors[1].Id,
                    EnrollmentStatusId = enrollmentStatuses[0].Id,
                    Image = "https://via.placeholder.com/150"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                            
                    StudentNumber = "2024" + Random.Shared.Next(100000, 999999).ToString(System.Globalization.CultureInfo.InvariantCulture),
                    OneTimeCode = 1234,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    FirstName = new MultilingualText { Ar = "سارة", En = "Sarah" },
                    LastName = new MultilingualText { Ar = "خالد", En = "Khaled" },
                    FatherName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    Year = 4,
                    MajorId = majors[2].Id,
                    EnrollmentStatusId = enrollmentStatuses[0].Id,
                    Image = "https://via.placeholder.com/150"
                },

                // Electrical Engineering Students
                new()
                {
                    Id = Guid.NewGuid(),
                    StudentNumber = "2024" + Random.Shared.Next(100000, 999999).ToString(System.Globalization.CultureInfo.InvariantCulture),
                    OneTimeCode = 1234,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    FirstName = new MultilingualText { Ar = "عمر", En = "Omar" },
                    LastName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    FatherName = new MultilingualText { Ar = "علي", En = "Ali" },
                    Year = 2,
                    MajorId = majors[4].Id,
                    EnrollmentStatusId = enrollmentStatuses[0].Id,
                    Image = "https://via.placeholder.com/150"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    StudentNumber = "2024" + Random.Shared.Next(100000, 999999).ToString(System.Globalization.CultureInfo.InvariantCulture),
                    OneTimeCode = 1234,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    FirstName = new MultilingualText { Ar = "ليلى", En = "Layla" },
                    LastName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    FatherName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    Year = 3,
                    MajorId = majors[5].Id,
                    EnrollmentStatusId = enrollmentStatuses[0].Id,
                    Image = "https://via.placeholder.com/150"
                },

                // Mechanical Engineering Students
                new()
                {
                    Id = Guid.NewGuid(),
                    StudentNumber = "2024" + Random.Shared.Next(100000, 999999).ToString(System.Globalization.CultureInfo.InvariantCulture),
                    OneTimeCode = 1234,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    FirstName = new MultilingualText { Ar = "خالد", En = "Khaled" },
                    LastName = new MultilingualText { Ar = "علي", En = "Ali" },
                    FatherName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    Year = 4,
                    MajorId = majors[4].Id,
                    EnrollmentStatusId = enrollmentStatuses[0].Id,
                    Image = "https://via.placeholder.com/150"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    StudentNumber = "2024" + Random.Shared.Next(100000, 999999).ToString(System.Globalization.CultureInfo.InvariantCulture),
                    OneTimeCode = 1234,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    FirstName = new MultilingualText { Ar = "نور", En = "Nour" },
                    LastName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    FatherName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    Year = 2,
                    MajorId = majors[4].Id,
                    EnrollmentStatusId = enrollmentStatuses[0].Id,
                    Image = "https://via.placeholder.com/150"
                },

                // Civil Engineering Students
                new()
                {
                    Id = Guid.NewGuid(),
                    StudentNumber = "2024" + Random.Shared.Next(100000, 999999).ToString(System.Globalization.CultureInfo.InvariantCulture),
                    OneTimeCode = 1234,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    FirstName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    LastName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    FatherName = new MultilingualText { Ar = "علي", En = "Ali" },
                    Year = 3,
                    MajorId = majors[8].Id,
                    EnrollmentStatusId = enrollmentStatuses[0].Id,
                    Image = "https://via.placeholder.com/150"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    StudentNumber = "2024" + Random.Shared.Next(100000, 999999).ToString(System.Globalization.CultureInfo.InvariantCulture),
                    OneTimeCode = 1234,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    FirstName = new MultilingualText { Ar = "سارة", En = "Sarah" },
                    LastName = new MultilingualText { Ar = "خالد", En = "Khaled" },
                    FatherName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    Year = 4,
                    MajorId = majors[9].Id,
                    EnrollmentStatusId = enrollmentStatuses[0].Id,
                    Image = "https://via.placeholder.com/150"
                },

                // Medical Students
                new()
                {
                    Id = Guid.NewGuid(),
                    StudentNumber = "2024" + Random.Shared.Next(100000, 999999).ToString(System.Globalization.CultureInfo.InvariantCulture),
                    OneTimeCode = 1234,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    FirstName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    LastName = new MultilingualText { Ar = "علي", En = "Ali" },
                    FatherName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    Year = 3,
                    MajorId = majors[12].Id,
                    EnrollmentStatusId = enrollmentStatuses[0].Id,
                    Image = "https://via.placeholder.com/150"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    StudentNumber = "2024" + Random.Shared.Next(100000, 999999).ToString(System.Globalization.CultureInfo.InvariantCulture),
                    OneTimeCode = 1234,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    FirstName = new MultilingualText { Ar = "فاطمة", En = "Fatima" },
                    LastName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    FatherName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    Year = 4,
                    MajorId = majors[12].Id,
                    EnrollmentStatusId = enrollmentStatuses[0].Id,
                    Image = "https://via.placeholder.com/150"
                }
            };

            await Context.StudentCredentials.AddRangeAsync(studentCredentials);
            await Context.SaveChangesAsync();
        }
    }
}
