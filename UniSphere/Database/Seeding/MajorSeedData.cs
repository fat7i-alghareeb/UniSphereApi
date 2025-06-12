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
                // Informatics Engineering Majors
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
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "هندسة الذكاء الاصطناعي", En = "Artificial Intelligence Engineering" },
                    FacultyId = faculties[0].Id,
                    NumberOfYears = 4
                },

                // Mechanical and Electrical Engineering Majors
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "هندسة الميكانيك", En = "Mechanical Engineering" },
                    FacultyId = faculties[1].Id,
                    NumberOfYears = 5
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "هندسة الكهرباء", En = "Electrical Engineering" },
                    FacultyId = faculties[1].Id,
                    NumberOfYears = 5
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "هندسة الإلكترون", En = "Electronics Engineering" },
                    FacultyId = faculties[1].Id,
                    NumberOfYears = 5
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "هندسة الاتصالات", En = "Communications Engineering" },
                    FacultyId = faculties[1].Id,
                    NumberOfYears = 5
                },

                // Civil Engineering Majors
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "هندسة الإنشاءات", En = "Structural Engineering" },
                    FacultyId = faculties[2].Id,
                    NumberOfYears = 5
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "هندسة المياه", En = "Water Engineering" },
                    FacultyId = faculties[2].Id,
                    NumberOfYears = 5
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "هندسة الطرق", En = "Road Engineering" },
                    FacultyId = faculties[2].Id,
                    NumberOfYears = 5
                },

                // Architecture Majors
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "العمارة", En = "Architecture" },
                    FacultyId = faculties[3].Id,
                    NumberOfYears = 5
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "التخطيط العمراني", En = "Urban Planning" },
                    FacultyId = faculties[3].Id,
                    NumberOfYears = 5
                },

                // Medical Majors
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "الطب البشري", En = "Human Medicine" },
                    FacultyId = faculties[4].Id,
                    NumberOfYears = 6
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "طب الأسنان", En = "Dentistry" },
                    FacultyId = faculties[5].Id,
                    NumberOfYears = 5
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "الصيدلة", En = "Pharmacy" },
                    FacultyId = faculties[6].Id,
                    NumberOfYears = 5
                },

                // Science Majors
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "الرياضيات", En = "Mathematics" },
                    FacultyId = faculties[7].Id,
                    NumberOfYears = 4
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "الفيزياء", En = "Physics" },
                    FacultyId = faculties[7].Id,
                    NumberOfYears = 4
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "الكيمياء", En = "Chemistry" },
                    FacultyId = faculties[7].Id,
                    NumberOfYears = 4
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "علوم الحياة", En = "Life Sciences" },
                    FacultyId = faculties[7].Id,
                    NumberOfYears = 4
                }
            };

            await Context.Majors.AddRangeAsync(majors);
            await Context.SaveChangesAsync();

            // Add enrollment statuses
            var enrollmentStatuses = new List<EnrollmentStatus>
            {
                new() { Name = new MultilingualText { Ar = "منتظم", En = "Regular" } },
                new() { Name = new MultilingualText { Ar = "محول", En = "Transferred" } },
                new() { Name = new MultilingualText { Ar = "مستمع", En = "Auditor" } },
                new() { Name = new MultilingualText { Ar = "موازي", En = "Parallel" } }
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
                },
                new()
                {
                    FirstName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    LastName = new MultilingualText { Ar = "خالد", En = "Khaled" }
                },
                new()
                {
                    FirstName = new MultilingualText { Ar = "فاطمة", En = "Fatima" },
                    LastName = new MultilingualText { Ar = "حسين", En = "Hussein" }
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
                },
                new()
                {
                    Name = new MultilingualText { Ar = "الذكاء الاصطناعي", En = "Artificial Intelligence" },
                    Description = new MultilingualText { Ar = "مقدمة في الذكاء الاصطناعي", En = "Introduction to Artificial Intelligence" },
                    MajorId = majors[3].Id,
                    IsLabRequired = true,
                    IsMultipleChoice = false,
                    IsOpenBook = false,
                    MidtermGrade = 30,
                    FinalGrade = 70
                },
                new()
                {
                    Name = new MultilingualText { Ar = "تحليل الدوائر الكهربائية", En = "Electrical Circuit Analysis" },
                    Description = new MultilingualText { Ar = "تحليل الدوائر الكهربائية الأساسية", En = "Basic Electrical Circuit Analysis" },
                    MajorId = majors[5].Id,
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
                new() { SubjectId = subjects[1].Id, ProfessorId = professors[1].Id },
                new() { SubjectId = subjects[2].Id, ProfessorId = professors[2].Id },
                new() { SubjectId = subjects[3].Id, ProfessorId = professors[3].Id }
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
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FacultyId = faculties[1].Id,
                    Email = "student3@example.com",
                    FirstName = new MultilingualText { Ar = "علي", En = "Ali" },
                    LastName = new MultilingualText { Ar = "حسين", En = "Hussein" },
                    FatherName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    Year = 3,
                    MajorId = majors[4].Id,
                    EnrollmentStatusId = enrollmentStatuses[0].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FacultyId = faculties[4].Id,
                    Email = "student4@example.com",
                    FirstName = new MultilingualText { Ar = "سارة", En = "Sarah" },
                    LastName = new MultilingualText { Ar = "خالد", En = "Khaled" },
                    FatherName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    Year = 4,
                    MajorId = majors[12].Id,
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
                },
                new()
                {
                    SubjectId = subjects[2].Id,
                    StudentId = studentCredentials[1].Id,
                    FacultyId = studentCredentials[1].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                },
                new()
                {
                    SubjectId = subjects[3].Id,
                    StudentId = studentCredentials[2].Id,
                    FacultyId = studentCredentials[2].FacultyId,
                    AttemptNumber = 1,
                    IsCurrentlyEnrolled = true
                }
            };

            await Context.SubjectStudentLinks.AddRangeAsync(subjectStudentLinks);
            await Context.SaveChangesAsync();
        }
    }
}
