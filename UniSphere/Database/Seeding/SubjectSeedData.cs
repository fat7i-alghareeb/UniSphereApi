using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class SubjectSeedData(ApplicationDbContext context) : SeedData(context)
{

    public override async Task SeedAsync()
    {
        if (!await Context.Subjects.AnyAsync())
        {
            List<Major> majors = await Context.Majors.ToListAsync();
            if(majors.Count == 0)
            {
                return;
            }
            var subjects = new List<Subject>
            {
                new()
                {
                    Id = Guid.NewGuid(),
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
                    Id = Guid.NewGuid(),
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
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "برمجة متقدمة", En = "Advanced Programming" },
                    Description = new MultilingualText { Ar = "مفاهيم البرمجة المتقدمة", En = "Advanced Programming Concepts" },
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
        }
    }
} 
