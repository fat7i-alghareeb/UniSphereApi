using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class EnrollmentStatusSeedData(ApplicationDbContext context) : SeedData(context)
{


    public override async Task SeedAsync()
    {
        if (!await Context.EnrollmentStatuses.AnyAsync())
        {
            var enrollmentStatuses = new List<EnrollmentStatus>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "منتظم", En = "Regular" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "محول", En = "Transferred" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = new MultilingualText { Ar = "مستمع", En = "Auditor" }
                }
            };

            await Context.EnrollmentStatuses.AddRangeAsync(enrollmentStatuses);
            await Context.SaveChangesAsync();
        }
    }
} 