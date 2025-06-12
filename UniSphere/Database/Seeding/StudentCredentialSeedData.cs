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
            if(faculties.Count == 0 || majors.Count == 0 || enrollmentStatuses.Count == 0)
            {
                return;
            }
            var studentCredentials = new List<StudentCredential>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    FacultyId = faculties[0].Id,
                    Email = "student1@example.com",
                    FirstName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    LastName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    FatherName = new MultilingualText { Ar = "علي", En = "Ali" },
                    Year = 2,
                    MajorId = majors[0].Id,
                    EnrollmentStatusId = enrollmentStatuses[0].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FacultyId = faculties[0].Id,
                    Email = "student2@example.com",
                    FirstName = new MultilingualText { Ar = "فاطمة", En = "Fatima" },
                    LastName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    FatherName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    Year = 2,
                    MajorId = majors[0].Id,
                    EnrollmentStatusId = enrollmentStatuses[0].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FacultyId = faculties[0].Id,
                    Email = "student3@example.com",
                    FirstName = new MultilingualText { Ar = "علي", En = "Ali" },
                    LastName = new MultilingualText { Ar = "حسين", En = "Hussein" },
                    FatherName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    Year = 3,
                    MajorId = majors[1].Id,
                    EnrollmentStatusId = enrollmentStatuses[0].Id
                }
            };

            await Context.StudentCredentials.AddRangeAsync(studentCredentials);
            await Context.SaveChangesAsync();
        }
    }
} 