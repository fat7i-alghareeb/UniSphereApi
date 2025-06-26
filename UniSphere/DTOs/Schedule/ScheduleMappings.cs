using UniSphere.Api.Controllers;
using UniSphere.Api.Entities;

namespace UniSphere.Api.DTOs.Schedule;

internal static class ScheduleMappings
{
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
        // Create a list of all days in the month
        var daysInMonth = new List<DayScheduleDto>();
        var currentDate = month;
        
        while (currentDate.Month == month.Month)
        {
            var dayLectures = new List<DayLectureDto>();
            
            // Get all schedules for this specific day
            var daySchedules = schedules.Where(s => s.ScheduleDate == currentDate).ToList();
            
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

    public static CreateScheduleDto ToCreateScheduleDto(this Entities.Schedule schedule)
    {
        return new CreateScheduleDto
        {
            MajorId = schedule.MajorId,
            Year = schedule.Year,
            ScheduleDate = schedule.ScheduleDate,
            Lectures = schedule.Lectures.Select(l => new CreateLectureDto
            {
                SubjectNameEn = l.SubjectName.En ?? "",
                SubjectNameAr = l.SubjectName.Ar ?? "",
                LecturerNameEn = l.LecturerName.En ?? "",
                LecturerNameAr = l.LecturerName.Ar ?? "",
                StartTime = l.StartTime,
                EndTime = l.EndTime,
                LectureHallEn = l.LectureHall.En ?? "",
                LectureHallAr = l.LectureHall.Ar ?? ""
            }).ToList()
        };
    }

    public static Entities.Schedule ToSchedule(this CreateScheduleDto dto)
    {
        return new Entities.Schedule
        {
            Id = Guid.NewGuid(),
            MajorId = dto.MajorId,
            Year = dto.Year,
            ScheduleDate = dto.ScheduleDate,
            Lectures = dto.Lectures.Select(l => new Lecture
            {
                Id = Guid.NewGuid(),
                SubjectName = new MultilingualText { En = l.SubjectNameEn, Ar = l.SubjectNameAr },
                LecturerName = new MultilingualText { En = l.LecturerNameEn, Ar = l.LecturerNameAr },
                StartTime = l.StartTime,
                EndTime = l.EndTime,
                LectureHall = new MultilingualText { En = l.LectureHallEn, Ar = l.LectureHallAr }
            }).ToList()
        };
    }

    public static Lecture ToLecture(this CreateLectureDto dto, Guid scheduleId)
    {
        return new Lecture
        {
            Id = Guid.NewGuid(),
            ScheduleId = scheduleId,
            SubjectName = new MultilingualText { En = dto.SubjectNameEn, Ar = dto.SubjectNameAr },
            LecturerName = new MultilingualText { En = dto.LecturerNameEn, Ar = dto.LecturerNameAr },
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            LectureHall = new MultilingualText { En = dto.LectureHallEn, Ar = dto.LectureHallAr }
        };
    }

    public static Lecture ToLecture(this AddLectureDto dto, Guid scheduleId)
    {
        return new Lecture
        {
            Id = Guid.NewGuid(),
            ScheduleId = scheduleId,
            SubjectName = new MultilingualText { En = dto.SubjectNameEn, Ar = dto.SubjectNameAr },
            LecturerName = new MultilingualText { En = dto.LecturerNameEn, Ar = dto.LecturerNameAr },
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            LectureHall = new MultilingualText { En = dto.LectureHallEn, Ar = dto.LectureHallAr }
        };
    }
}
