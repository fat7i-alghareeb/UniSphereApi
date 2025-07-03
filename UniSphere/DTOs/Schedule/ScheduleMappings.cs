using UniSphere.Api.Controllers;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Schedule;

internal static class ScheduleMappings
{
    public static DayLectureDto ToDayLectureDto(this Lecture lecture, Languages lang)
    {
        return new DayLectureDto
        {
            Id = lecture.Id,
            SubjectName = lecture.Subject.Name.GetTranslatedString(lang),
            LectureName = lecture.Subject.Name.GetTranslatedString(lang),
            ProfessorName = $"{lecture.Professor.FirstName.GetTranslatedString(lang)} {lecture.Professor.LastName.GetTranslatedString(lang)}",
            LectureHall = lecture.LectureHall.GetTranslatedString(lang),
            StartTime = lecture.StartTime,
            EndTime = lecture.EndTime,
            SubjectId = lecture.SubjectId,
            ProfessorId = lecture.ProfessorId
        };
    }

    public static CreateLectureDto ToCreateLectureDto(this Lecture lecture)
    {
        return new CreateLectureDto
        {
            Id = lecture.Id,
            SubjectId = lecture.SubjectId,
            StartTime = lecture.StartTime,
            EndTime = lecture.EndTime,
            LectureHallEn = lecture.LectureHall.En ?? string.Empty,
            LectureHallAr = lecture.LectureHall.Ar ?? string.Empty
        };
    }

    // public static MonthScheduleDto ToMonthScheduleDto(this Entities.Schedule schedule, DateOnly month,Languages lang)
    // {
    //     // Create a list of all days in the month
    //     var daysInMonth = new List<DayScheduleDto>();
    //     var currentDate = month;
    //     
    //     while (currentDate.Month == month.Month)
    //     {
    //         // Check if this schedule belongs to the current day
    //         var scheduleDate = DateOnly.FromDateTime(schedule.ScheduleDate.ToUniversalTime());
    //         var dayLectures = new List<DayLectureDto>();
    //         
    //         if (scheduleDate == currentDate)
    //         {
    //             // Get all lectures for this schedule (they all belong to the same date)
    //             dayLectures = schedule.Lectures
    //                 .Select(l => new DayLectureDto
    //                 {
    //                     SubjectName = l.SubjectName.GetTranslatedString(lang) ,
    //                     LectureName = l.SubjectName.GetTranslatedString(lang) ,
    //                     LectureHall = l.LectureHall.GetTranslatedString(lang) ,
    //                     StartTime = l.StartTime,
    //                     EndTime = l.EndTime
    //                 })
    //                 .ToList();
    //         }
    //         
    //         daysInMonth.Add(new DayScheduleDto
    //         {
    //             Date = currentDate,
    //             Lectures = dayLectures
    //         });
    //         
    //         currentDate = currentDate.AddDays(1);
    //     }
    //
    //     return new MonthScheduleDto
    //     {
    //         Month = month,
    //         Days = daysInMonth
    //     };
    // }

    public static MonthScheduleDto CombineSchedulesIntoMonth(this List<Entities.Schedule> schedules, DateOnly month ,Languages lang)
    {
        // Create a list of days that have schedules
        var daysInMonth = new List<DayScheduleDto>();
        var currentDate = month;
        
        while (currentDate.Month == month.Month)
        {
            var dayLectures = new List<DayLectureDto>();
            
            // Get all schedules for this specific day
            var daySchedules = schedules.Where(s => s.ScheduleDate == currentDate).ToList();
            
            // Only add the day if there are actual schedules for it
            if (daySchedules.Any())
            {
                // Combine all lectures from all schedules for this day
                foreach (var schedule in daySchedules)
                {
                    var scheduleLectures = schedule.Lectures
                        .Select(l => l.ToDayLectureDto(lang));
                    
                    dayLectures.AddRange(scheduleLectures);
                }
                
                // Use the first schedule's ID
                var scheduleId = daySchedules[0].Id;
                
                daysInMonth.Add(new DayScheduleDto
                {
                    Date = currentDate,
                    ScheduleId = scheduleId,
                    Lectures = dayLectures
                });
            }
            
            currentDate = currentDate.AddDays(1);
        }

        return new MonthScheduleDto
        {
            Month = month,
            Days = daysInMonth
        };
    }

    public static CreateScheduleDto ToCreateScheduleDto(this Entities.Schedule schedule)
    {
        return new CreateScheduleDto
        {
            Year = schedule.Year,
            ScheduleDate = schedule.ScheduleDate,
        };
    }

    public static Entities.Schedule ToSchedule(this CreateScheduleDto dto)
    {
        return new Entities.Schedule
        {
            Id = Guid.NewGuid(),
            Year = dto.Year,
            ScheduleDate = dto.ScheduleDate,
        };
    }

    public static Lecture ToLecture(this CreateLectureDto dto, Guid scheduleId, Guid professorId)
    {
        return new Lecture
        {
            Id = dto.Id ?? Guid.NewGuid(),
            ScheduleId = scheduleId,
            SubjectId = dto.SubjectId,
            ProfessorId = professorId,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            LectureHall = new MultilingualText { En = dto.LectureHallEn, Ar = dto.LectureHallAr }
        };
    }

    public static Lecture UpdateFromDto(this Lecture lecture, CreateLectureDto dto)
    {
        lecture.SubjectId = dto.SubjectId;
        lecture.StartTime = dto.StartTime;
        lecture.EndTime = dto.EndTime;
        lecture.LectureHall = new MultilingualText { En = dto.LectureHallEn, Ar = dto.LectureHallAr };
        return lecture;
    }
}
