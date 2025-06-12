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
                // Morning Lectures
                new()
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = schedules[0].Id,
                    SubjectName = new MultilingualText { Ar = "برمجة الويب", En = "Web Programming" },
                    LecturerName = new MultilingualText { Ar = "أحمد محمد", En = "Ahmed Mohammed" },
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    LectureHall = new MultilingualText { Ar = "القاعة 101", En = "Hall 101" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = schedules[0].Id,
                    SubjectName = new MultilingualText { Ar = "قواعد البيانات", En = "Databases" },
                    LecturerName = new MultilingualText { Ar = "سارة علي", En = "Sarah Ali" },
                    StartTime = new TimeSpan(10, 30, 0),
                    EndTime = new TimeSpan(12, 30, 0),
                    LectureHall = new MultilingualText { Ar = "القاعة 202", En = "Hall 202" }
                },

                // Afternoon Lectures
                new()
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = schedules[1].Id,
                    SubjectName = new MultilingualText { Ar = "الذكاء الاصطناعي", En = "Artificial Intelligence" },
                    LecturerName = new MultilingualText { Ar = "محمد خالد", En = "Mohammed Khaled" },
                    StartTime = new TimeSpan(12, 0, 0),
                    EndTime = new TimeSpan(14, 0, 0),
                    LectureHall = new MultilingualText { Ar = "القاعة 303", En = "Hall 303" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = schedules[1].Id,
                    SubjectName = new MultilingualText { Ar = "معمارية الحاسوب", En = "Computer Architecture" },
                    LecturerName = new MultilingualText { Ar = "علي حسين", En = "Ali Hussein" },
                    StartTime = new TimeSpan(14, 30, 0),
                    EndTime = new TimeSpan(16, 30, 0),
                    LectureHall = new MultilingualText { Ar = "القاعة 404", En = "Hall 404" }
                },

                // Evening Lectures
                new()
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = schedules[2].Id,
                    SubjectName = new MultilingualText { Ar = "الدوائر الكهربائية", En = "Electric Circuits" },
                    LecturerName = new MultilingualText { Ar = "فاطمة أحمد", En = "Fatima Ahmed" },
                    StartTime = new TimeSpan(16, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    LectureHall = new MultilingualText { Ar = "القاعة 505", En = "Hall 505" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = schedules[2].Id,
                    SubjectName = new MultilingualText { Ar = "ميكانيكا التربة", En = "Soil Mechanics" },
                    LecturerName = new MultilingualText { Ar = "خالد محمد", En = "Khaled Mohammed" },
                    StartTime = new TimeSpan(18, 30, 0),
                    EndTime = new TimeSpan(20, 30, 0),
                    LectureHall = new MultilingualText { Ar = "القاعة 606", En = "Hall 606" }
                }
            };
            await Context.Lectures.AddRangeAsync(lectures);
            await Context.SaveChangesAsync();
        }
    }
} 
