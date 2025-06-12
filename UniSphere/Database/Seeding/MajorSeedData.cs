using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class MajorSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.Majors.AnyAsync())
        {
            List<Faculty> faculties = await Context.Faculties.ToListAsync();
            if(faculties.Count == 0)
            {
                return;
            }
            var majors = new List<Major>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "هندسة البرمجيات", En = "Software Engineering" },
                    FacultyId = faculties[0].Id,
                    NumberOfYears = 4
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "هندسة الشبكات", En = "Network Engineering" },
                    FacultyId = faculties[0].Id,
                    NumberOfYears = 4
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "هندسة الحاسوب", En = "Computer Engineering" },
                    FacultyId = faculties[0].Id,
                    NumberOfYears = 4
                }
            };

            await Context.Majors.AddRangeAsync(majors);
            await Context.SaveChangesAsync();

            // Add enrollment statuses
            var enrollmentStatuses = new List<EnrollmentStatus>
            {
                new() { Name = new MultilingualText { Ar = "منتظم", En = "Regular" } },
                new() { Name = new MultilingualText { Ar = "محول", En = "Transferred" } }
            };

            await Context.EnrollmentStatuses.AddRangeAsync(enrollmentStatuses);
            await Context.SaveChangesAsync();

            // Add professors
            var professors = new List<Professor>
            {
                new()
                {
                    FirstName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    LastName = new MultilingualText { Ar = "محمد", En = "Mohammed" }
                },
                new()
                {
                    FirstName = new MultilingualText { Ar = "سارة", En = "Sarah" },
                    LastName = new MultilingualText { Ar = "علي", En = "Ali" }
                }
            };

            await Context.Professors.AddRangeAsync(professors);
            await Context.SaveChangesAsync();

            // Add subjects
            var subjects = new List<Subject>
            {
                new()
                {
                    Name = new MultilingualText { Ar = "برمجة الويب", En = "Web Programming" },
                    Description = new MultilingualText { Ar = "مقدمة في تطوير تطبيقات الويب", En = "Introduction to Web Application Development" },
                    MajorId = majors[0].Id,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Name = new MultilingualText { Ar = "قواعد البيانات", En = "Databases" },
                    Description = new MultilingualText { Ar = "مقدمة في قواعد البيانات", En = "Introduction to Databases" },
                    MajorId = majors[0].Id,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                }
            };

            await Context.Subjects.AddRangeAsync(subjects);
            await Context.SaveChangesAsync();

            // Add subject-professor links
            var subjectProfessorLinks = new List<SubjectProfessorLink>
            {
                new() { SubjectId = subjects[0].Id, ProfessorId = professors[0].Id },
                new() { SubjectId = subjects[1].Id, ProfessorId = professors[1].Id }
            };

            await Context.SubjectProfessorLinks.AddRangeAsync(subjectProfessorLinks);
            await Context.SaveChangesAsync();

            // Add student credentials
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
                }
            };

            await Context.StudentCredentials.AddRangeAsync(studentCredentials);
            await Context.SaveChangesAsync();

            // Add subject-student links
            var subjectStudentLinks = new List<SubjectStudentLink>
            {
                new()
                {
                    SubjectId = subjects[0].Id,
                    StudentId = studentCredentials[0].Id,
                    FacultyId = studentCredentials[0].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                },
                new()
                {
                    SubjectId = subjects[1].Id,
                    StudentId = studentCredentials[0].Id,
                    FacultyId = studentCredentials[0].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                }
            };

            await Context.SubjectStudentLinks.AddRangeAsync(subjectStudentLinks);
            await Context.SaveChangesAsync();
        }
    }
}
