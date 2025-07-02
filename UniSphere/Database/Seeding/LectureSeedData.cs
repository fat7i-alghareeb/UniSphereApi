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
            List<Subject> subjects = await Context.Subjects.ToListAsync();
            List<Professor> professors = await Context.Professors.ToListAsync();
            
            if(schedules.Count == 0 || subjects.Count == 0 || professors.Count == 0)
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
                    SubjectId = subjects[0].Id,
                    ProfessorId = professors[0].Id,
                    StartTime = new TimeSpan(8, 0, 0),
                    EndTime = new TimeSpan(10, 0, 0),
                    LectureHall = new MultilingualText { Ar = "القاعة 101", En = "Hall 101" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = schedules[0].Id,
                    SubjectId = subjects[1].Id,
                    ProfessorId = professors[1].Id,
                    StartTime = new TimeSpan(10, 30, 0),
                    EndTime = new TimeSpan(12, 30, 0),
                    LectureHall = new MultilingualText { Ar = "القاعة 202", En = "Hall 202" }
                },

                // Afternoon Lectures
                new()
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = schedules[1].Id,
                    SubjectId = subjects[2].Id,
                    ProfessorId = professors[2].Id,
                    StartTime = new TimeSpan(12, 0, 0),
                    EndTime = new TimeSpan(14, 0, 0),
                    LectureHall = new MultilingualText { Ar = "القاعة 303", En = "Hall 303" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = schedules[1].Id,
                    SubjectId = subjects[3].Id,
                    ProfessorId = professors[3].Id,
                    StartTime = new TimeSpan(14, 30, 0),
                    EndTime = new TimeSpan(16, 30, 0),
                    LectureHall = new MultilingualText { Ar = "القاعة 404", En = "Hall 404" }
                },

                // Evening Lectures
                new()
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = schedules[2].Id,
                    SubjectId = subjects[4].Id,
                    ProfessorId = professors[4].Id,
                    StartTime = new TimeSpan(16, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    LectureHall = new MultilingualText { Ar = "القاعة 505", En = "Hall 505" }
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    ScheduleId = schedules[2].Id,
                    SubjectId = subjects[5].Id,
                    ProfessorId = professors[5].Id,
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
