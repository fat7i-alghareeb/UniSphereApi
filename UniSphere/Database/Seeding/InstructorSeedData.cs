using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class InstructorSeedData(ApplicationDbContext context) : SeedData(context)
{

    public override async Task SeedAsync()
    {
        if (!await Context.Instructors.AnyAsync())
        {
            var instructors = new List<Instructor>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = new MultilingualText { Ar = "رامي", En = "Rami" },
                    LastName = new MultilingualText { Ar = "خالد", En = "Khaled" },
                    FatherName = new MultilingualText { Ar = "سامي", En = "Sami" },
                    Email = "rami.khaled@example.com",
                    Password = "password1"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = new MultilingualText { Ar = "دينا", En = "Dina" },
                    LastName = new MultilingualText { Ar = "سعيد", En = "Saeed" },
                    FatherName = new MultilingualText { Ar = "فارس", En = "Fares" },
                    Email = "dina.saeed@example.com",
                    Password = "password2"
                }
            };
            await Context.Instructors.AddRangeAsync(instructors);
            await Context.SaveChangesAsync();
        }
    }
} 