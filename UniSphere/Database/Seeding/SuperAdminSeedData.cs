using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class SuperAdminSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.SuperAdmins.AnyAsync())
        {
            var faculties = await Context.Faculties.ToListAsync();
            var superAdmins = new List<SuperAdmin>
            {
                new()
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1"),
                    FirstName = new MultilingualText { Ar = "ليلى", En = "Layla" },
                    LastName = new MultilingualText { Ar = "حسن", En = "Hassan" },
                    Gmail = "layla.hassan@unisphere.com",
                    OneTimeCode = 4321,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    FacultyId = faculties[0].Id
                },
                new()
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2"),
                    FirstName = new MultilingualText { Ar = "نور", En = "Noor" },
                    LastName = new MultilingualText { Ar = "محمد", En = "Mohammed" },
                    Gmail = "noor.mohammed@unisphere.com",
                    OneTimeCode = 8765,
                    OneTimeCodeCreatedDate = DateTime.UtcNow,
                    OneTimeCodeExpirationInMinutes = 10000,
                    FacultyId = faculties[1].Id
                }
            };
            await Context.SuperAdmins.AddRangeAsync(superAdmins);
            await Context.SaveChangesAsync();
        }
    }
} 
