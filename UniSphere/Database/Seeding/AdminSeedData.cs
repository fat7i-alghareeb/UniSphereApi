using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class AdminSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.Admins.AnyAsync())
        {
            var majors = await Context.Majors.ToListAsync();
            var admins = new List<Admin>
            {
                new()
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1"),
                    FirstName = new MultilingualText { Ar = "أحمد", En = "Ahmed" },
                    LastName = new MultilingualText { Ar = "خالد", En = "Khaled" },
                    Gmail = "ahmed.khaled@unisphere.com",
                    OneTimeCode = 1234,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    MajorId = majors[0].Id
                },
                new()
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                    FirstName = new MultilingualText { Ar = "سارة", En = "Sarah" },
                    LastName = new MultilingualText { Ar = "علي", En = "Ali" },
                    Gmail = "sarah.ali@unisphere.com",
                    OneTimeCode = 5678,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    MajorId = majors[1].Id
                }
            };
            await Context.Admins.AddRangeAsync(admins);
            await Context.SaveChangesAsync();
        }
    }
} 