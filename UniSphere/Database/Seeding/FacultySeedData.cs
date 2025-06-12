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
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة المعلوماتية", En = "Faculty of Informatics Engineering" },
                    UniversityId = universities[0].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة الميكانيكية", En = "Faculty of Mechanical Engineering" },
                    UniversityId = universities[0].Id
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "كلية الهندسة الكهربائية", En = "Faculty of Electrical Engineering" },
                    UniversityId = universities[1].Id
                }
            };

            await Context.Faculties.AddRangeAsync(faculties);
            await Context.SaveChangesAsync();
        }
    }
}
