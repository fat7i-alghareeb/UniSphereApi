using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Schedule;
using UniSphere.Api.Entities;
using UniSphere.Api.Extensions;
using UniSphere.Api.Helpers;
using Microsoft.AspNetCore.JsonPatch;

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
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        // Verify admin has access to this major
        var admin = await dbContext.Admins
            .FirstOrDefaultAsync(a => a.Id == adminId);

        if (admin is null || admin.MajorId != createDto.MajorId)
        {
            return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
        }

        // Check if schedule already exists for this date, major, and year
        var existingSchedule = await dbContext.Schedules
            .FirstOrDefaultAsync(s => s.MajorId == createDto.MajorId && 
                                     s.Year == createDto.Year && 
                                     s.ScheduleDate == createDto.ScheduleDate);

        if (existingSchedule != null)
        {
            return BadRequest(new { message = BilingualErrorMessages.GetScheduleAlreadyExistsMessage(Lang) });
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
            // Ensure ScheduleId is set
            // Find the professor for the subject
            var professorId = await dbContext.SubjectProfessorLinks
                .Where(spl => spl.SubjectId == lectureDto.SubjectId)
                .Select(spl => spl.ProfessorId)
                .FirstOrDefaultAsync();
            if (professorId == Guid.Empty)
            {
                return BadRequest(new { message = $"No professor assigned to subject {lectureDto.SubjectId}." });
            }
            schedule.Lectures.Add(lectureDto.ToLecture(schedule.Id, professorId));
        }

        dbContext.Schedules.Add(schedule);
        await dbContext.SaveChangesAsync();

        // Return the created schedule in the same format as the student endpoints
        var monthSchedule = new List<Schedule> { schedule }.CombineSchedulesIntoMonth(createDto.ScheduleDate, Lang);
        return Ok(monthSchedule);
    }

    [HttpPost("AddLecture")]
    public async Task<ActionResult<DayScheduleDto>> AddLecture([Required] Guid scheduleId, CreateLectureDto addDto)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        // Verify admin has access to this schedule
        var schedule = await dbContext.Schedules
            .Include(s => s.Major)
            .FirstOrDefaultAsync(s => s.Id == scheduleId);

        if (schedule is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetScheduleNotFoundMessage(Lang) });
        }

        var admin = await dbContext.Admins
            .FirstOrDefaultAsync(a => a.Id == adminId);

        if (admin is null || admin.MajorId != schedule.MajorId)
        {
            return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
        }

        // Find the professor for the subject
        var professorId = await dbContext.SubjectProfessorLinks
            .Where(spl => spl.SubjectId == addDto.SubjectId)
            .Select(spl => spl.ProfessorId)
            .FirstOrDefaultAsync();
        if (professorId == Guid.Empty)
        {
            return BadRequest(new { message = "No professor assigned to this subject." });
        }

        var lecture = addDto.ToLecture(scheduleId, professorId);

        dbContext.Lectures.Add(lecture);
        await dbContext.SaveChangesAsync();

        // Return the updated day schedule
        var updatedSchedule = await dbContext.Schedules
            .Include(s => s.Lectures)
            .ThenInclude(l => l.Subject)
            .Include(s => s.Lectures)
            .ThenInclude(l => l.Professor)
            .FirstOrDefaultAsync(s => s.Id == scheduleId);

        if (updatedSchedule is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetScheduleNotFoundMessage(Lang) });
        }

        var dayLectures = updatedSchedule.Lectures
            .Select(l => new DayLectureDto
            {
                Id = l.Id,
                SubjectName = l.Subject.Name.GetTranslatedString(Lang),
                LectureName = l.Subject.Name.GetTranslatedString(Lang),
                ProfessorName = $"{l.Professor.FirstName.GetTranslatedString(Lang)} {l.Professor.LastName.GetTranslatedString(Lang)}",
                LectureHall = l.LectureHall.GetTranslatedString(Lang),
                StartTime = l.StartTime,
                EndTime = l.EndTime,
                SubjectId = l.SubjectId,
                ProfessorId = l.ProfessorId
            })
            .ToList();

        return Ok(new DayScheduleDto
        {
            Date = updatedSchedule.ScheduleDate,
            ScheduleId = updatedSchedule.Id,
            Lectures = dayLectures
        });
    }

    [HttpDelete("DeleteLecture/{lectureId:guid}")]
    public async Task<ActionResult> DeleteLecture(Guid lectureId)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var lecture = await dbContext.Lectures
            .Include(l => l.Schedule)
            .ThenInclude(s => s.Major)
            .FirstOrDefaultAsync(l => l.Id == lectureId);

        if (lecture is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetLectureNotFoundMessage(Lang) });
        }

        var admin = await dbContext.Admins
            .FirstOrDefaultAsync(a => a.Id == adminId);

        if (admin is null || admin.MajorId != lecture.Schedule.MajorId)
        {
            return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
        }

        dbContext.Lectures.Remove(lecture);
        await dbContext.SaveChangesAsync();

        return Ok(new { message = Lang == Languages.En ? "Lecture deleted successfully" : "تم حذف المحاضرة بنجاح" });
    }

    [HttpGet("GetSchedule/{scheduleId:guid}")]
    public async Task<ActionResult<DayScheduleDto>> GetSchedule(Guid scheduleId)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var schedule = await dbContext.Schedules
            .Include(s => s.Lectures)
            .ThenInclude(l => l.Subject)
            .Include(s => s.Lectures)
            .ThenInclude(l => l.Professor)
            .FirstOrDefaultAsync(s => s.Id == scheduleId);

        if (schedule is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetScheduleNotFoundMessage(Lang) });
        }

        var admin = await dbContext.Admins
            .FirstOrDefaultAsync(a => a.Id == adminId);

        if (admin is null || admin.MajorId != schedule.MajorId)
        {
            return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
        }

        var dayLectures = schedule.Lectures
            .Select(l => new DayLectureDto
            {
                Id = l.Id,
                SubjectName = l.Subject.Name.GetTranslatedString(Lang),
                LectureName = l.Subject.Name.GetTranslatedString(Lang),
                ProfessorName = $"{l.Professor.FirstName.GetTranslatedString(Lang)} {l.Professor.LastName.GetTranslatedString(Lang)}",
                LectureHall = l.LectureHall.GetTranslatedString(Lang),
                StartTime = l.StartTime,
                EndTime = l.EndTime,
                SubjectId = l.SubjectId,
                ProfessorId = l.ProfessorId
            })
            .ToList();

        return Ok(new DayScheduleDto
        {
            Date = schedule.ScheduleDate,
            ScheduleId = schedule.Id,
            Lectures = dayLectures
        });
    }

    [HttpPatch("UpdateLecture/{lectureId:guid}")]
    public async Task<ActionResult<DayScheduleDto>> UpdateLecture(Guid lectureId, JsonPatchDocument<CreateLectureDto> patchDocument)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var lecture = await dbContext.Lectures
            .Include(l => l.Schedule)
            .ThenInclude(s => s.Major)
            .FirstOrDefaultAsync(l => l.Id == lectureId);

        if (lecture is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetLectureNotFoundMessage(Lang) });
        }

        var admin = await dbContext.Admins
            .FirstOrDefaultAsync(a => a.Id == adminId);

        if (admin is null || admin.MajorId != lecture.Schedule.MajorId)
        {
            return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
        }

        // Convert lecture to DTO for patching
        var lectureDto = new CreateLectureDto
        {
            Id = lecture.Id,
            SubjectId = lecture.SubjectId,
            StartTime = lecture.StartTime,
            EndTime = lecture.EndTime,
            LectureHallEn = lecture.LectureHall.En ?? "",
            LectureHallAr = lecture.LectureHall.Ar ?? ""
        };

        // Apply the patch
        patchDocument.ApplyTo(lectureDto, ModelState);
        if (!TryValidateModel(lectureDto))
        {
            return ValidationProblem(ModelState);
        }

        // If SubjectId changed, update ProfessorId accordingly
        if (lecture.SubjectId != lectureDto.SubjectId)
        {
            var newProfessorId = await dbContext.SubjectProfessorLinks
                .Where(spl => spl.SubjectId == lectureDto.SubjectId)
                .Select(spl => spl.ProfessorId)
                .FirstOrDefaultAsync();
            if (newProfessorId == Guid.Empty)
            {
                return BadRequest(new { message = "No professor assigned to the new subject." });
            }
            lecture.ProfessorId = newProfessorId;
        }

        // Update the lecture with patched values using the mapping method
        lecture.UpdateFromDto(lectureDto);

        await dbContext.SaveChangesAsync();

        // Get all lectures for this schedule to return the complete day schedule
        var updatedSchedule = await dbContext.Schedules
            .Include(s => s.Lectures)
            .ThenInclude(l => l.Subject)
            .Include(s => s.Lectures)
            .ThenInclude(l => l.Professor)
            .FirstOrDefaultAsync(s => s.Id == lecture.ScheduleId);

        if (updatedSchedule is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetScheduleNotFoundMessage(Lang) });
        }

        var dayLectures = updatedSchedule.Lectures
            .Select(l => new DayLectureDto
            {
                Id = l.Id,
                SubjectName = l.Subject.Name.GetTranslatedString(Lang),
                LectureName = l.Subject.Name.GetTranslatedString(Lang),
                ProfessorName = $"{l.Professor.FirstName.GetTranslatedString(Lang)} {l.Professor.LastName.GetTranslatedString(Lang)}",
                LectureHall = l.LectureHall.GetTranslatedString(Lang),
                StartTime = l.StartTime,
                EndTime = l.EndTime,
                SubjectId = l.SubjectId,
                ProfessorId = l.ProfessorId
            })
            .ToList();

        return Ok(new DayScheduleDto
        {
            Date = updatedSchedule.ScheduleDate,
            ScheduleId = updatedSchedule.Id,
            Lectures = dayLectures
        });
    }
} 