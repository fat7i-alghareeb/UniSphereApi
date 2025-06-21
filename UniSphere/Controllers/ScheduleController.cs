using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Schedule;
using UniSphere.Api.Extensions;

namespace UniSphere.Api.Controllers;

[Authorize]
[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public sealed class ScheduleController(ApplicationDbContext dbContext) : BaseController
{
    [HttpGet("GetMySchedule")]
    public async Task<ActionResult<MonthScheduleDto>> GetMySchedule()
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            return Unauthorized();
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
            return Unauthorized();
        }

        var currentMonth = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
        var startDate = currentMonth.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var endDate = currentMonth.AddMonths(1).AddDays(-1).ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        var schedules = await dbContext.Schedules
            .Where(s => s.Year == studentInfo.Year &&
                        s.MajorId == studentInfo.MajorId &&
                        s.ScheduleDate >= startDate &&
                        s.ScheduleDate <= endDate)
            .Include(s => s.Lectures)
            .ToListAsync();

        if (!schedules.Any())
        {
            return NotFound("No schedule found for the current month");
        }

        var monthSchedule = schedules.CombineSchedulesIntoMonth(currentMonth, Lang);
        return Ok(monthSchedule);
    }

    [HttpGet("GetScheduleByMonth")]
    public async Task<ActionResult<MonthScheduleDto>> GetScheduleByMonth([Required] int month, [Required] int year)
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            return Unauthorized();
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
            return Unauthorized();
        }

        var targetMonth = new DateOnly(year, month, 1);
        var startDate = targetMonth.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var endDate = targetMonth.AddMonths(1).AddDays(-1).ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

        var schedules = await dbContext.Schedules
            .Where(s => s.Year == studentInfo.Year &&
                        s.MajorId == studentInfo.MajorId &&
                        s.ScheduleDate >= startDate &&
                        s.ScheduleDate <= endDate)
            .Include(s => s.Lectures)
            .ToListAsync();

        if (!schedules.Any())
        {
            return NotFound("No schedule found for the target month");
        }

        var monthSchedule = schedules.CombineSchedulesIntoMonth(targetMonth, Lang);
        return Ok(monthSchedule);
    }

    [HttpGet("GetAvailableLabs")]
    public async Task<ActionResult<AvailableLabsCollectionDto>> GetAvailableLabs()
    {
        var studentId = HttpContext.User.GetStudentId();
        if (studentId is null)
        {
            return Unauthorized();
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
}
