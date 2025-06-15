using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Controllers;
 
[Authorize]
[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public class StudentController (ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet("GetMe")]
     [ProducesResponseType(StatusCodes.Status200OK)]   
    public async Task<ActionResult<BaseStudentDto>> GetMe()
    {
        string? studentStringId = HttpContext.User.FindFirstValue("studentId");
        if (string.IsNullOrWhiteSpace(studentStringId) || !Guid.TryParse(studentStringId, out Guid studentId))
        {
            return Unauthorized();
        }

        StudentCredential studentCredential = await dbContext.StudentCredentials
            .Include(sc => sc.EnrollmentStatus)
            .Include(sc => sc.Major)
            .FirstOrDefaultAsync(sc => sc.Id == studentId);
        if (studentCredential is null)
        {
            return Unauthorized();
        }
        return Ok(studentCredential.ToBaseStudentDto());
    }
}
