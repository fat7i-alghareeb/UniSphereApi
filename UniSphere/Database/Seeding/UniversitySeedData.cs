using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class UniversitySeedData(ApplicationDbContext context) : SeedData (context)
{

    public override async Task SeedAsync()
    {
        if (!await Context.Universities.AnyAsync())
        {
            var universities = new List<University>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "جامعة دمشق", En = "Damascus University" },
                    Type = new MultilingualText { Ar = "حكومية", En = "Public" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "جامعة حلب", En = "Aleppo University" },
                    Type = new MultilingualText { Ar = "حكومية", En = "Public" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "جامعة تشرين", En = "Tishreen University" },
                    Type = new MultilingualText { Ar = "حكومية", En = "Public" }
                }
            };

            await Context.Universities.AddRangeAsync(universities);
            await Context.SaveChangesAsync();
        }
    }
}
