using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class FacultySeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.Faculties.AnyAsync())
        {
            List<University> universities = await Context.Universities.ToListAsync();
            if(universities.Count == 0)
            {
                return;
            }

            var faculties = new List<Faculty>
            {
                // Damascus University Faculties
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة المعلوماتية", En = "Faculty of Informatics Engineering" },
                    UniversityId = universities[0].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة الميكانيكية والكهربائية", En = "Faculty of Mechanical and Electrical Engineering" },
                    UniversityId = universities[0].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة المدنية", En = "Faculty of Civil Engineering" },
                    UniversityId = universities[0].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة المعمارية", En = "Faculty of Architecture" },
                    UniversityId = universities[0].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الطب", En = "Faculty of Medicine" },
                    UniversityId = universities[0].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية طب الأسنان", En = "Faculty of Dentistry" },
                    UniversityId = universities[0].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الصيدلة", En = "Faculty of Pharmacy" },
                    UniversityId = universities[0].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية العلوم", En = "Faculty of Science" },
                    UniversityId = universities[0].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الاقتصاد", En = "Faculty of Economics" },
                    UniversityId = universities[0].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الحقوق", En = "Faculty of Law" },
                    UniversityId = universities[0].Id
                },

                // Aleppo University Faculties
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة المعلوماتية", En = "Faculty of Informatics Engineering" },
                    UniversityId = universities[1].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة الميكانيكية", En = "Faculty of Mechanical Engineering" },
                    UniversityId = universities[1].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة الكهربائية", En = "Faculty of Electrical Engineering" },
                    UniversityId = universities[1].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الطب", En = "Faculty of Medicine" },
                    UniversityId = universities[1].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية العلوم", En = "Faculty of Science" },
                    UniversityId = universities[1].Id
                },

                // Tishreen University Faculties
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة الميكانيكية والكهربائية", En = "Faculty of Mechanical and Electrical Engineering" },
                    UniversityId = universities[2].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة المدنية", En = "Faculty of Civil Engineering" },
                    UniversityId = universities[2].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة المعمارية", En = "Faculty of Architecture" },
                    UniversityId = universities[2].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الطب", En = "Faculty of Medicine" },
                    UniversityId = universities[2].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية العلوم", En = "Faculty of Science" },
                    UniversityId = universities[2].Id
                }
            };

            await Context.Faculties.AddRangeAsync(faculties);
            await Context.SaveChangesAsync();
        }
    }
}
