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
                    Id = Guid.Parse("09da2b33-d994-4a4f-9271-5056165a7146"),
                    Name = new MultilingualText { Ar = "هندسة البرمجيات", En = "Software Engineering" },
                    FacultyId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    NumberOfYears = 4
                },
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444441"),
                    Name = new MultilingualText { Ar = "هندسة الشبكات", En = "Network Engineering" },
                    FacultyId = faculties[0].Id,
                    NumberOfYears = 4
                },
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444442"),
                    Name = new MultilingualText { Ar = "هندسة الحاسوب", En = "Computer Engineering" },
                    FacultyId = faculties[0].Id,
                    NumberOfYears = 4
                },
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444443"),
                    Name = new MultilingualText { Ar = "هندسة الذكاء الاصطناعي", En = "Artificial Intelligence Engineering" },
                    FacultyId = faculties[0].Id,
                    NumberOfYears = 4
                },

                // Mechanical and Electrical Engineering Majors
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = new MultilingualText { Ar = "هندسة الميكانيك", En = "Mechanical Engineering" },
                    FacultyId = faculties[1].Id,
                    NumberOfYears = 5
                },
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444445"),
                    Name = new MultilingualText { Ar = "هندسة الكهرباء", En = "Electrical Engineering" },
                    FacultyId = faculties[1].Id,
                    NumberOfYears = 5
                },
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444446"),
                    Name = new MultilingualText { Ar = "هندسة الإلكترون", En = "Electronics Engineering" },
                    FacultyId = faculties[1].Id,
                    NumberOfYears = 5
                },
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444447"),
                    Name = new MultilingualText { Ar = "هندسة الاتصالات", En = "Communications Engineering" },
                    FacultyId = faculties[1].Id,
                    NumberOfYears = 5
                },

                // Civil Engineering Majors
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444448"),
                    Name = new MultilingualText { Ar = "هندسة الإنشاءات", En = "Structural Engineering" },
                    FacultyId = faculties[2].Id,
                    NumberOfYears = 5
                },
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444449"),
                    Name = new MultilingualText { Ar = "هندسة المياه", En = "Water Engineering" },
                    FacultyId = faculties[2].Id,
                    NumberOfYears = 5
                },
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-44444444444a"),
                    Name = new MultilingualText { Ar = "هندسة الطرق", En = "Road Engineering" },
                    FacultyId = faculties[2].Id,
                    NumberOfYears = 5
                },

                // Architecture Majors
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-44444444444b"),
                    Name = new MultilingualText { Ar = "العمارة", En = "Architecture" },
                    FacultyId = faculties[3].Id,
                    NumberOfYears = 5
                },
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-44444444444c"),
                    Name = new MultilingualText { Ar = "التخطيط العمراني", En = "Urban Planning" },
                    FacultyId = faculties[3].Id,
                    NumberOfYears = 5
                },

                // Medical Majors
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-44444444444d"),
                    Name = new MultilingualText { Ar = "الطب البشري", En = "Human Medicine" },
                    FacultyId = faculties[4].Id,
                    NumberOfYears = 6
                },
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-44444444444e"),
                    Name = new MultilingualText { Ar = "طب الأسنان", En = "Dentistry" },
                    FacultyId = faculties[5].Id,
                    NumberOfYears = 5
                },
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-44444444444f"),
                    Name = new MultilingualText { Ar = "الصيدلة", En = "Pharmacy" },
                    FacultyId = faculties[6].Id,
                    NumberOfYears = 5
                },

                // Science Majors
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444450"),
                    Name = new MultilingualText { Ar = "الرياضيات", En = "Mathematics" },
                    FacultyId = faculties[7].Id,
                    NumberOfYears = 4
                },
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444451"),
                    Name = new MultilingualText { Ar = "الفيزياء", En = "Physics" },
                    FacultyId = faculties[7].Id,
                    NumberOfYears = 4
                },
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444452"),
                    Name = new MultilingualText { Ar = "الكيمياء", En = "Chemistry" },
                    FacultyId = faculties[7].Id,
                    NumberOfYears = 4
                },
                new()
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444453"),
                    Name = new MultilingualText { Ar = "علوم الحياة", En = "Life Sciences" },
                    FacultyId = faculties[7].Id,
                    NumberOfYears = 4
                }
            };

            await Context.Majors.AddRangeAsync(majors);
            await Context.SaveChangesAsync();
        }
    }
}
