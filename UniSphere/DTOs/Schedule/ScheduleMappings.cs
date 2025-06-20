using UniSphere.Api.Controllers;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Schedule;

internal static class ScheduleMappings
{
    public static MonthScheduleDto ToMonthScheduleDto(this Entities.Schedule schedule, DateOnly month,Languages lang)
    {
        // Create a list of all days in the month
        var daysInMonth = new List<DayScheduleDto>();
        var currentDate = month;
        
        while (currentDate.Month == month.Month)
        {
            // Check if this schedule belongs to the current day
            var scheduleDate = DateOnly.FromDateTime(schedule.ScheduleDate.ToUniversalTime());
            var dayLectures = new List<DayLectureDto>();
            
            if (scheduleDate == currentDate)
            {
                // Get all lectures for this schedule (they all belong to the same date)
                dayLectures = schedule.Lectures
                    .Select(l => new DayLectureDto
                    {
                        SubjectName = l.SubjectName.GetTranslatedString(lang) ,
                        LectureName = l.SubjectName.GetTranslatedString(lang) ,
                        LectureHall = l.LectureHall.GetTranslatedString(lang) ,
                        StartTime = l.StartTime,
                        EndTime = l.EndTime
                    })
                    .ToList();
            }
            
            daysInMonth.Add(new DayScheduleDto
            {
                Date = currentDate,
                Lectures = dayLectures
            });
            
            currentDate = currentDate.AddDays(1);
        }

        return new MonthScheduleDto
        {
            Month = month,
            Days = daysInMonth
        };
    }

    public static MonthScheduleDto CombineSchedulesIntoMonth(this List<Entities.Schedule> schedules, DateOnly month ,Languages lang)
    {
        // Create a list of all days in the month
        var daysInMonth = new List<DayScheduleDto>();
        var currentDate = month;
        
        while (currentDate.Month == month.Month)
        {
            var dayLectures = new List<DayLectureDto>();
            
            // Get all schedules for this specific day
            var daySchedules = schedules.Where(s => 
                DateOnly.FromDateTime(s.ScheduleDate.ToUniversalTime()) == currentDate).ToList();
            
            // Combine all lectures from all schedules for this day
            foreach (var schedule in daySchedules)
            {
                var scheduleLectures = schedule.Lectures
                    .Select(l => new DayLectureDto
                    {
                        SubjectName = l.SubjectName.GetTranslatedString(lang) ,
                        LectureName = l.SubjectName.GetTranslatedString(lang) ,
                        LectureHall = l.LectureHall.GetTranslatedString(lang) ,
                        StartTime = l.StartTime,
                        EndTime = l.EndTime
                    });
                
                dayLectures.AddRange(scheduleLectures);
            }
            
            daysInMonth.Add(new DayScheduleDto
            {
                Date = currentDate,
                Lectures = dayLectures
            });
            
            currentDate = currentDate.AddDays(1);
        }

        return new MonthScheduleDto
        {
            Month = month,
            Days = daysInMonth
        };
    }
}
