﻿using System.ComponentModel.DataAnnotations;
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
