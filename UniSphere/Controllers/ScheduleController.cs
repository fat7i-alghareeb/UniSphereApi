using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Schedule;
using UniSphere.Api.Entities;
using UniSphere.Api.Extensions;
using UniSphere.Api.Helpers;

namespace UniSphere.Api.Controllers;

[Authorize]
[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public sealed class ScheduleController(ApplicationDbContext dbContext) : BaseController
{
    // Student Endpoints
    [HttpGet("Student/GetMySchedule")]
    public async Task<ActionResult<MonthScheduleDto>> GetMySchedule()
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var studentInfo = await dbContext.StudentCredentials
            .Where(sc => sc.Id == studentId)
            .Select(sc => new
            {
                sc.Year,
                sc.MajorId
            })
            .FirstOrDefaultAsync();

        if (studentInfo is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var currentMonth = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var startDate = currentMonth;
        var endDate = currentMonth.AddMonths(1).AddDays(-1);

        var schedules = await dbContext.Schedules
            .Where(s => s.Year == studentInfo.Year &&
                        s.MajorId == studentInfo.MajorId &&
                        s.ScheduleDate >= startDate &&
                        s.ScheduleDate <= endDate)
            .Include(s => s.Lectures)
            .ThenInclude(l => l.Subject)
            .Include(s => s.Lectures)
            .ThenInclude(l => l.Professor)
            .ToListAsync();

        if (!schedules.Any())
        {
            return NotFound(new { message = Lang == Languages.En ? "No schedule found for the current month" : "لم يتم العثور على جدول دراسي للشهر الحالي" });
        }

        var monthSchedule = schedules.CombineSchedulesIntoMonth(currentMonth, Lang);
        return Ok(monthSchedule);
    }

    [HttpGet("Student/GetScheduleByMonth")]
    public async Task<ActionResult<MonthScheduleDto>> GetScheduleByMonth([Required] int month, [Required] int year)
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var studentInfo = await dbContext.StudentCredentials
            .Where(sc => sc.Id == studentId)
            .Select(sc => new
            {
                sc.Year,
                sc.MajorId
            })
            .FirstOrDefaultAsync();

        if (studentInfo is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var targetMonth = new DateOnly(year, month, 1);
        DateOnly startDate = targetMonth;
        var endDate = targetMonth.AddMonths(1).AddDays(-1);

        var schedules = await dbContext.Schedules
            .Where(s => s.Year == studentInfo.Year &&
                        s.MajorId == studentInfo.MajorId &&
                        s.ScheduleDate >= startDate &&
                        s.ScheduleDate <= endDate)
            .Include(s => s.Lectures)
            .ThenInclude(l => l.Subject)
            .Include(s => s.Lectures)
            .ThenInclude(l => l.Professor)
            .ToListAsync();

        if (!schedules.Any())
        {
            return NotFound(new { message = Lang == Languages.En ? "No schedule found for the target month" : "لم يتم العثور على جدول دراسي للشهر المحدد" });
        }

        var monthSchedule = schedules.CombineSchedulesIntoMonth(targetMonth, Lang);
        return Ok(monthSchedule);
    }

    [HttpGet("Student/GetAvailableLabs")]
    public async Task<ActionResult<AvailableLabsCollectionDto>> GetAvailableLabs()
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var majorId = await dbContext.StudentCredentials
            .Where(sc => sc.Id == studentId)
            .Select(sc => sc.MajorId)
            .FirstOrDefaultAsync();
        var availableLabs = await dbContext.Labs
            .Include(l => l.Subjects)
            .Where(l => l.Subjects.Any(s => s.MajorId == majorId))
            .Select(l => new AvailableLabDto
            {
                Id = l.Id,
                Name = l.Name.GetTranslatedString(Lang)
            })
            .ToListAsync();
        if (availableLabs.Count == 0)
        {
            return NoContent();
        }

        return Ok(new AvailableLabsCollectionDto
            {
                Labs = availableLabs
            }
            );
    }

    // Admin Endpoints
    [HttpGet("Admin/GetMySchedule")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MonthScheduleDto>> GetAdminMySchedule([FromQuery] int? year)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var admin = await dbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Id == adminId);
        if (admin is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var currentMonth = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var startDate = currentMonth;
        var endDate = currentMonth.AddMonths(1).AddDays(-1);

        var query = dbContext.Schedules
            .Where(s => s.MajorId == admin.MajorId &&
                        s.ScheduleDate >= startDate &&
                        s.ScheduleDate <= endDate);

        if (year.HasValue)
        {
            query = query.Where(s => s.Year == year.Value);
        }

        var schedules = await query
            .Include(s => s.Lectures)
            .ThenInclude(l => l.Subject)
            .Include(s => s.Lectures)
            .ThenInclude(l => l.Professor)
            .ToListAsync();

        if (!schedules.Any())
        {
            return NotFound(new { message = Lang == Languages.En ? "No schedule found for the current month" : "لم يتم العثور على جدول دراسي للشهر الحالي" });
        }

        var monthSchedule = schedules.CombineSchedulesIntoMonth(currentMonth, Lang);
        return Ok(monthSchedule);
    }

    [HttpGet("Admin/GetScheduleByMonth")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MonthScheduleDto>> GetAdminScheduleByMonth([Required] int month, [Required] int year , [Required] int majorYear)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var admin = await dbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Id == adminId);
        if (admin is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var targetMonth = new DateOnly(year, month, 1);
        DateOnly startDate = targetMonth;
        var endDate = targetMonth.AddMonths(1).AddDays(-1);

        var schedules = await dbContext.Schedules
            .Where(s => s.MajorId == admin.MajorId &&
                        s.Year == majorYear &&
                        s.ScheduleDate >= startDate &&
                        s.ScheduleDate <= endDate)
            .Include(s => s.Lectures)
            .ThenInclude(l => l.Subject)
            .Include(s => s.Lectures)
            .ThenInclude(l => l.Professor)
            .ToListAsync();

        if (!schedules.Any())
        {
            return NotFound(new { message = Lang == Languages.En ? "No schedule found for the target month" : "لم يتم العثور على جدول دراسي للشهر المحدد" });
        }

        var monthSchedule = schedules.CombineSchedulesIntoMonth(targetMonth, Lang);
        return Ok(monthSchedule);
    }

    // --- Admin Management Endpoints (from ScheduleManagementController) ---

    [HttpPost("Admin/CreateSchedule")]
    [Authorize(Roles = "Admin")]
    /// <summary>
    /// Creates a new schedule for the admin's major and year. Prevents duplicate schedules for the same date.
    /// </summary>
    public async Task<ActionResult<MonthScheduleDto>> CreateSchedule(CreateScheduleDto createDto)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
        if (admin is null)
        {
            return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
        }

        var existingSchedule = await dbContext.Schedules
            .FirstOrDefaultAsync(s => s.MajorId == admin.MajorId &&
                                     s.Year == createDto.Year &&
                                     s.ScheduleDate == createDto.ScheduleDate);
        if (existingSchedule != null)
        {
            return BadRequest(new { message = BilingualErrorMessages.GetScheduleAlreadyExistsMessage(Lang) });
        }

        var schedule = new Schedule
        {
            Id = Guid.NewGuid(),
            MajorId = admin.MajorId,
            Year = createDto.Year,
            ScheduleDate = createDto.ScheduleDate,
            Lectures = new List<Lecture>()
        };
        dbContext.Schedules.Add(schedule);
        await dbContext.SaveChangesAsync();
        var monthSchedule = new List<Schedule> { schedule }.CombineSchedulesIntoMonth(createDto.ScheduleDate, Lang);
        return Ok(monthSchedule);
    }

    [HttpPost("Admin/AddLecture")]
    [Authorize(Roles = "Admin")]
    /// <summary>
    /// Adds a lecture to a schedule, ensuring the professor is assigned to the subject.
    /// </summary>
    public async Task<ActionResult<DayScheduleDto>> AddLecture([Required] Guid scheduleId, CreateLectureDto addDto)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }
        var schedule = await dbContext.Schedules.Include(s => s.Major).FirstOrDefaultAsync(s => s.Id == scheduleId);
        if (schedule is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetScheduleNotFoundMessage(Lang) });
        }
        var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
        if (admin is null || admin.MajorId != schedule.MajorId)
        {
            return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
        }
        var professorId = await dbContext.SubjectProfessorLinks.Where(spl => spl.SubjectId == addDto.SubjectId).Select(spl => spl.ProfessorId).FirstOrDefaultAsync();
        if (professorId == Guid.Empty)
        {
            return BadRequest(new { message = "No professor assigned to this subject." });
        }
        var lecture = addDto.ToLecture(scheduleId, professorId);
        dbContext.Lectures.Add(lecture);
        await dbContext.SaveChangesAsync();
        var updatedSchedule = await dbContext.Schedules.Include(s => s.Lectures).ThenInclude(l => l.Subject).Include(s => s.Lectures).ThenInclude(l => l.Professor).FirstOrDefaultAsync(s => s.Id == scheduleId);
        if (updatedSchedule is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetScheduleNotFoundMessage(Lang) });
        }
        var dayLectures = updatedSchedule.Lectures.Select(l => new DayLectureDto
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
        }).ToList();
        return Ok(new DayScheduleDto
        {
            Date = updatedSchedule.ScheduleDate,
            ScheduleId = updatedSchedule.Id,
            Lectures = dayLectures
        });
    }

    [HttpDelete("Admin/DeleteLecture/{lectureId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteLecture(Guid lectureId)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }
        var lecture = await dbContext.Lectures.Include(l => l.Schedule).ThenInclude(s => s.Major).FirstOrDefaultAsync(l => l.Id == lectureId);
        if (lecture is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetLectureNotFoundMessage(Lang) });
        }
        var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
        if (admin is null || admin.MajorId != lecture.Schedule.MajorId)
        {
            return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
        }
        dbContext.Lectures.Remove(lecture);
        await dbContext.SaveChangesAsync();
        return Ok(new { message = Lang == Languages.En ? "Lecture deleted successfully" : "تم حذف المحاضرة بنجاح" });
    }

    [HttpGet("Admin/GetSchedule/{scheduleId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DayScheduleDto>> GetSchedule(Guid scheduleId)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }
        var schedule = await dbContext.Schedules.Include(s => s.Lectures).ThenInclude(l => l.Subject).Include(s => s.Lectures).ThenInclude(l => l.Professor).FirstOrDefaultAsync(s => s.Id == scheduleId);
        if (schedule is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetScheduleNotFoundMessage(Lang) });
        }
        var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
        if (admin is null || admin.MajorId != schedule.MajorId)
        {
            return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
        }
        var dayLectures = schedule.Lectures.Select(l => new DayLectureDto
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
        }).ToList();
        return Ok(new DayScheduleDto
        {
            Date = schedule.ScheduleDate,
            ScheduleId = schedule.Id,
            Lectures = dayLectures
        });
    }

    [HttpPatch("Admin/UpdateLecture/{lectureId:guid}")]
    [Authorize(Roles = "Admin")]
    /// <summary>
    /// Updates a lecture using a JSON patch document. Handles professor reassignment if subject changes.
    /// </summary>
    public async Task<ActionResult<DayScheduleDto>> UpdateLecture(Guid lectureId, JsonPatchDocument<CreateLectureDto> patchDocument)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }
        var lecture = await dbContext.Lectures.Include(l => l.Schedule).ThenInclude(s => s.Major).FirstOrDefaultAsync(l => l.Id == lectureId);
        if (lecture is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetLectureNotFoundMessage(Lang) });
        }
        var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
        if (admin is null || admin.MajorId != lecture.Schedule.MajorId)
        {
            return Forbid(BilingualErrorMessages.GetForbiddenMessage(Lang));
        }
        var lectureDto = new CreateLectureDto
        {
            Id = lecture.Id,
            SubjectId = lecture.SubjectId,
            StartTime = lecture.StartTime,
            EndTime = lecture.EndTime,
            LectureHallEn = lecture.LectureHall.En ?? "",
            LectureHallAr = lecture.LectureHall.Ar ?? ""
        };
        patchDocument.ApplyTo(lectureDto, ModelState);
        if (!TryValidateModel(lectureDto))
        {
            return ValidationProblem(ModelState);
        }
        if (lecture.SubjectId != lectureDto.SubjectId)
        {
            var newProfessorId = await dbContext.SubjectProfessorLinks.Where(spl => spl.SubjectId == lectureDto.SubjectId).Select(spl => spl.ProfessorId).FirstOrDefaultAsync();
            if (newProfessorId == Guid.Empty)
            {
                return BadRequest(new { message = "No professor assigned to the new subject." });
            }
            lecture.ProfessorId = newProfessorId;
        }
        lecture.UpdateFromDto(lectureDto);
        await dbContext.SaveChangesAsync();
        var updatedSchedule = await dbContext.Schedules.Include(s => s.Lectures).ThenInclude(l => l.Subject).Include(s => s.Lectures).ThenInclude(l => l.Professor).FirstOrDefaultAsync(s => s.Id == lecture.ScheduleId);
        if (updatedSchedule is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetScheduleNotFoundMessage(Lang) });
        }
        var dayLectures = updatedSchedule.Lectures.Select(l => new DayLectureDto
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
        }).ToList();
        return Ok(new DayScheduleDto
        {
            Date = updatedSchedule.ScheduleDate,
            ScheduleId = updatedSchedule.Id,
            Lectures = dayLectures
        });
    }

    // [HttpPatch("{id:guid}")]
    // [Authorize(Roles = "Admin")]
    // public async Task<ActionResult<MonthScheduleDto>> UpdateSchedule(Guid id, JsonPatchDocument<CreateScheduleDto> patchDocument)
    // {
    //     var adminId = HttpContext.User.GetAdminId();
    //     if (adminId is null)
    //     {
    //         return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
    //     }

    //     var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
    //     if (admin is null)
    //     {
    //         return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
    //     }

    //     var schedule = await dbContext.Schedules
    //         .Include(s => s.Lectures)
    //         .ThenInclude(l => l.Subject)
    //         .Include(s => s.Lectures)
    //         .ThenInclude(l => l.Professor)
    //         .FirstOrDefaultAsync(s => s.Id == id && s.MajorId == admin.MajorId);
    //     if (schedule is null)
    //     {
    //         return NotFound(new { message = BilingualErrorMessages.GetScheduleNotFoundMessage(Lang) });
    //     }

    //     var scheduleDto = schedule.ToCreateScheduleDto();
    //     patchDocument.ApplyTo(scheduleDto, ModelState);
    //     if (!TryValidateModel(scheduleDto))
    //     {
    //         return ValidationProblem(ModelState);
    //     }

    //     // Update the schedule
    //     schedule.Year = scheduleDto.Year;
    //     schedule.ScheduleDate = scheduleDto.ScheduleDate;

    //     // Update lectures
    //     dbContext.Lectures.RemoveRange(schedule.Lectures);
    //     schedule.Lectures.Clear();

    //     foreach (var lectureDto in scheduleDto.Lectures)
    //     {
    //         var professorId = await dbContext.SubjectProfessorLinks
    //             .Where(spl => spl.SubjectId == lectureDto.SubjectId)
    //             .Select(spl => spl.ProfessorId)
    //             .FirstOrDefaultAsync();
    //         if (professorId == Guid.Empty)
    //         {
    //             return BadRequest(new { message = $"No professor assigned to subject {lectureDto.SubjectId}." });
    //         }
    //         var lecture = lectureDto.ToLecture(schedule.Id, professorId);
    //         schedule.Lectures.Add(lecture);
    //     }

    //     await dbContext.SaveChangesAsync();

    //     // Return the updated schedule
    //     var monthSchedule = new List<Schedule> { schedule }.CombineSchedulesIntoMonth(schedule.ScheduleDate, Lang);
    //     return Ok(monthSchedule);
    // }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteSchedule(Guid id)
    {
        var adminId = HttpContext.User.GetAdminId();
        if (adminId is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Id == adminId);
        if (admin is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }

        var schedule = await dbContext.Schedules
            .FirstOrDefaultAsync(s => s.Id == id && s.MajorId == admin.MajorId);
        if (schedule is null)
        {
            return NotFound(new { message = BilingualErrorMessages.GetScheduleNotFoundMessage(Lang) });
        }

        dbContext.Schedules.Remove(schedule);
        await dbContext.SaveChangesAsync();
        return Ok(new { message = Lang == Languages.En ? "Schedule deleted successfully" : "تم حذف الجدول الدراسي بنجاح" });
    }
}

// public sealed record  AddLabsToScheduleDto
// {
//     public required Guid LabId { get; init; }
    
// }
