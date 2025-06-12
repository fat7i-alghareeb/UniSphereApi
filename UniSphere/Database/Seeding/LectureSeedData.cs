using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Database.Seeding;

public class LectureSeedData(ApplicationDbContext context) : SeedData(context)
{
    public override async Task SeedAsync()
    {
        if (!await Context.Lectures.AnyAsync())
        {
            List<Schedule> schedules = await Context.Schedules.ToListAsync();
            if(schedules.Count == 0)
            {
                return;
            }
            var lectures = new List<Lecture>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = schedules[0].Id,
                    SubjectName = new MultilingualText { Ar = "برمجة الويب", En = "Web Programming" },
                    LecturerName = new MultilingualText { Ar = "أحمد محمد", En = "Ahmed Mohammed" },
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(11, 0, 0),
                    LectureHall = new MultilingualText { Ar = "القاعة 101", En = "Hall 101" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = schedules[1].Id,
                    SubjectName = new MultilingualText { Ar = "قواعد البيانات", En = "Databases" },
                    LecturerName = new MultilingualText { Ar = "سارة علي", En = "Sarah Ali" },
                    StartTime = new TimeSpan(12, 0, 0),
                    EndTime = new TimeSpan(14, 0, 0),
                    LectureHall = new MultilingualText { Ar = "القاعة 202", En = "Hall 202" }
                }
            };
            await Context.Lectures.AddRangeAsync(lectures);
            await Context.SaveChangesAsync();
        }
    }
} 
