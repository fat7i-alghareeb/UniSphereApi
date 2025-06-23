using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Schedule;
using UniSphere.Api.Entities;
using UniSphere.Api.Extensions;

namespace UniSphere.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public class ScheduleManagementController(ApplicationDbContext dbContext) : BaseController
{
    [HttpPost("CreateSchedule")]
    public async Task<ActionResult<MonthScheduleDto>> CreateSchedule(CreateScheduleDto createDto)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized();
        }

        // Verify admin has access to this major
        var admin = await dbContext.Admins
            .FirstOrDefaultAsync(a => a.Id == adminId);

        if (admin is null || admin.MajorId != createDto.MajorId)
        {
            return Forbid();
        }

        // Check if schedule already exists for this date, major, and year
        var existingSchedule = await dbContext.Schedules
            .FirstOrDefaultAsync(s => s.MajorId == createDto.MajorId && 
                                     s.Year == createDto.Year && 
                                     s.ScheduleDate == createDto.ScheduleDate);

        if (existingSchedule != null)
        {
            return BadRequest("A schedule already exists for this date, major, and year");
        }

        var schedule = new Schedule
        {
            Id = Guid.NewGuid(),
            MajorId = createDto.MajorId,
            Year = createDto.Year,
            ScheduleDate = createDto.ScheduleDate,
            Lectures = new List<Lecture>()
        };

        // Add lectures to the schedule
        foreach (var lectureDto in createDto.Lectures)
        {
            var lecture = new Lecture
            {
                Id = Guid.NewGuid(),
                ScheduleId = schedule.Id,
                SubjectName = new MultilingualText { En = lectureDto.SubjectNameEn, Ar = lectureDto.SubjectNameAr },
                LecturerName = new MultilingualText { En = lectureDto.LecturerNameEn, Ar = lectureDto.LecturerNameAr },
                StartTime = lectureDto.StartTime,
                EndTime = lectureDto.EndTime,
                LectureHall = new MultilingualText { En = lectureDto.LectureHallEn, Ar = lectureDto.LectureHallAr }
            };
            schedule.Lectures.Add(lecture);
        }

        dbContext.Schedules.Add(schedule);
        await dbContext.SaveChangesAsync();

        // Return the created schedule in the same format as the student endpoints
        var monthSchedule = new List<Schedule> { schedule }.CombineSchedulesIntoMonth(createDto.ScheduleDate, Lang);
        return Ok(monthSchedule);
    }

    [HttpPost("AddLecture")]
    public async Task<ActionResult<DayScheduleDto>> AddLecture(AddLectureDto addDto)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized();
        }

        // Verify admin has access to this schedule
        var schedule = await dbContext.Schedules
            .Include(s => s.Major)
            .FirstOrDefaultAsync(s => s.Id == addDto.ScheduleId);

        if (schedule is null)
        {
            return NotFound("Schedule not found");
        }

        var admin = await dbContext.Admins
            .FirstOrDefaultAsync(a => a.Id == adminId);

        if (admin is null || admin.MajorId != schedule.MajorId)
        {
            return Forbid();
        }

        var lecture = new Lecture
        {
            Id = Guid.NewGuid(),
            ScheduleId = addDto.ScheduleId,
            SubjectName = new MultilingualText { En = addDto.SubjectNameEn, Ar = addDto.SubjectNameAr },
            LecturerName = new MultilingualText { En = addDto.LecturerNameEn, Ar = addDto.LecturerNameAr },
            StartTime = addDto.StartTime,
            EndTime = addDto.EndTime,
            LectureHall = new MultilingualText { En = addDto.LectureHallEn, Ar = addDto.LectureHallAr }
        };

        dbContext.Lectures.Add(lecture);
        await dbContext.SaveChangesAsync();

        // Return the updated day schedule
        var updatedSchedule = await dbContext.Schedules
            .Include(s => s.Lectures)
            .FirstOrDefaultAsync(s => s.Id == addDto.ScheduleId);

        if (updatedSchedule is null)
        {
            return NotFound();
        }

        var dayLectures = updatedSchedule.Lectures
            .Select(l => new DayLectureDto
            {
                SubjectName = l.SubjectName.GetTranslatedString(Lang),
                LectureName = l.SubjectName.GetTranslatedString(Lang),
                LectureHall = l.LectureHall.GetTranslatedString(Lang),
                StartTime = l.StartTime,
                EndTime = l.EndTime
            })
            .ToList();

        return Ok(new DayScheduleDto
        {
            Date = updatedSchedule.ScheduleDate,
            Lectures = dayLectures
        });
    }

    [HttpDelete("DeleteLecture/{lectureId:guid}")]
    public async Task<ActionResult> DeleteLecture(Guid lectureId)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized();
        }

        var lecture = await dbContext.Lectures
            .Include(l => l.Schedule)
            .ThenInclude(s => s.Major)
            .FirstOrDefaultAsync(l => l.Id == lectureId);

        if (lecture is null)
        {
            return NotFound("Lecture not found");
        }

        var admin = await dbContext.Admins
            .FirstOrDefaultAsync(a => a.Id == adminId);

        if (admin is null || admin.MajorId != lecture.Schedule.MajorId)
        {
            return Forbid();
        }

        dbContext.Lectures.Remove(lecture);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("GetSchedule/{scheduleId:guid}")]
    public async Task<ActionResult<DayScheduleDto>> GetSchedule(Guid scheduleId)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized();
        }

        var schedule = await dbContext.Schedules
            .Include(s => s.Lectures)
            .FirstOrDefaultAsync(s => s.Id == scheduleId);

        if (schedule is null)
        {
            return NotFound("Schedule not found");
        }

        var admin = await dbContext.Admins
            .FirstOrDefaultAsync(a => a.Id == adminId);

        if (admin is null || admin.MajorId != schedule.MajorId)
        {
            return Forbid();
        }

        var dayLectures = schedule.Lectures
            .Select(l => new DayLectureDto
            {
                SubjectName = l.SubjectName.GetTranslatedString(Lang),
                LectureName = l.SubjectName.GetTranslatedString(Lang),
                LectureHall = l.LectureHall.GetTranslatedString(Lang),
                StartTime = l.StartTime,
                EndTime = l.EndTime
            })
            .ToList();

        return Ok(new DayScheduleDto
        {
            Date = schedule.ScheduleDate,
            Lectures = dayLectures
        });
    }
} 