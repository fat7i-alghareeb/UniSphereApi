using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class ScheduleSeedData(ApplicationDbContext context) : SeedData(context)
{

    public override async Task SeedAsync()
    {
        if (!await Context.Schedules.AnyAsync())
        {
            List<Major> majors = await Context.Majors.ToListAsync();
            if(majors.Count == 0)
            {
                return;
            }
            var schedules = new List<Schedule>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    MajorId = majors[0].Id,
                    Year = 2,
                    ScheduleDate = DateTime.UtcNow.Date
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    MajorId = majors[1].Id,
                    Year = 3,
                    ScheduleDate = DateTime.UtcNow.Date.AddDays(1)
                }
            };
            await Context.Schedules.AddRangeAsync(schedules);
            await Context.SaveChangesAsync();
        }
    }
} 