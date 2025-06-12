using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class LabSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.Labs.AnyAsync())
        {
            var labs = new List<Lab>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "مخبر البرمجة", En = "Programming Lab" },
                    Description = new MultilingualText { Ar = "مخبر لتعليم البرمجة", En = "Lab for teaching programming" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "مخبر الشبكات", En = "Networks Lab" },
                    Description = new MultilingualText { Ar = "مخبر لتعليم الشبكات", En = "Lab for teaching networks" }
                }
            };
            await Context.Labs.AddRangeAsync(labs);
            await Context.SaveChangesAsync();
        }
    }
} 
