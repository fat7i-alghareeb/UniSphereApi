using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class ProfessorSeedData(ApplicationDbContext context) : SeedData(context)
{

    public override async Task SeedAsync()
    {
        if (!await Context.Professors.AnyAsync())
        {
            var professors = new List<Professor>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    LastName = new MultilingualText { Ar = "محمد", En = "Mohammed" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = new MultilingualText { Ar = "سارة", En = "Sarah" },
                    LastName = new MultilingualText { Ar = "علي", En = "Ali" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    LastName = new MultilingualText { Ar = "حسين", En = "Hussein" }
                }
            };

            await Context.Professors.AddRangeAsync(professors);
            await Context.SaveChangesAsync();
        }
    }
} 