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
                // Informatics Engineering Schedules
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
                    MajorId = majors[0].Id,
                    Year = 3,
                    ScheduleDate = DateTime.UtcNow.Date.AddDays(1)
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    MajorId = majors[0].Id,
                    Year = 4,
                    ScheduleDate = DateTime.UtcNow.Date.AddDays(2)
                },

                // Computer Engineering Schedules
                new()
                {
                    Id = Guid.NewGuid(),
                    MajorId = majors[1].Id,
                    Year = 2,
                    ScheduleDate = DateTime.UtcNow.Date
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    MajorId = majors[1].Id,
                    Year = 3,
                    ScheduleDate = DateTime.UtcNow.Date.AddDays(1)
                },

                // Electrical Engineering Schedules
                new()
                {
                    Id = Guid.NewGuid(),
                    MajorId = majors[4].Id,
                    Year = 2,
                    ScheduleDate = DateTime.UtcNow.Date
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    MajorId = majors[4].Id,
                    Year = 3,
                    ScheduleDate = DateTime.UtcNow.Date.AddDays(1)
                },

                // Civil Engineering Schedules
                new()
                {
                    Id = Guid.NewGuid(),
                    MajorId = majors[8].Id,
                    Year = 2,
                    ScheduleDate = DateTime.UtcNow.Date
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    MajorId = majors[8].Id,
                    Year = 3,
                    ScheduleDate = DateTime.UtcNow.Date.AddDays(1)
                },

                // Medical Schedules
                new()
                {
                    Id = Guid.NewGuid(),
                    MajorId = majors[12].Id,
                    Year = 2,
                    ScheduleDate = DateTime.UtcNow.Date
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    MajorId = majors[12].Id,
                    Year = 3,
                    ScheduleDate = DateTime.UtcNow.Date.AddDays(1)
                }
            };
            await Context.Schedules.AddRangeAsync(schedules);
            await Context.SaveChangesAsync();
        }
    }
} 