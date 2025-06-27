using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class AdminSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.Admins.AnyAsync())
        {
            // Check if the required major exists
            var softwareEngineeringMajor = await Context.Majors
                .FirstOrDefaultAsync(m => m.Id == Guid.Parse("09da2b33-d994-4a4f-9271-5056165a7146"));
            
            if (softwareEngineeringMajor == null)
            {
                // Major doesn't exist, skip admin seeding
                return;
            }

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
                    MajorId = softwareEngineeringMajor.Id
                },
                new()
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2"),
                    FirstName = new MultilingualText { Ar = "سارة", En = "Sarah" },
                    LastName = new MultilingualText { Ar = "علي", En = "Ali" },
                    Gmail = "sarah.ali@unisphere.com",
                    OneTimeCode = 1234,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    MajorId = softwareEngineeringMajor.Id
                }
            };
            await Context.Admins.AddRangeAsync(admins);
            await Context.SaveChangesAsync();
        }
    }
} 
