using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class SuperAdminSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.SuperAdmins.AnyAsync())
        {
            // Get the Informatics Engineering faculty (first faculty with hardcoded ID)
            var informaticsFaculty = await Context.Faculties
                .FirstOrDefaultAsync(f => f.Id == Guid.Parse("11111111-1111-1111-1111-111111111111"));
            
            if (informaticsFaculty == null)
            {
                // Faculty doesn't exist, skip super admin seeding
                return;
            }

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
                    FacultyId = informaticsFaculty.Id
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
                    FacultyId = informaticsFaculty.Id
                }
            };
            await Context.SuperAdmins.AddRangeAsync(superAdmins);
            await Context.SaveChangesAsync();
        }
    }
} 
