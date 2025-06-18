using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.Entities;
using UniSphere.Api.Extensions;

namespace UniSphere.Api.Controllers;
 
[ApiController]
[Route("api/[controller]")]
public class StudentController (ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet("GetMe")]
     [ProducesResponseType(StatusCodes.Status200OK)]   
    public async Task<ActionResult<BaseStudentDto>> GetMe()
    {
        var studentId = HttpContext.User.GetStudentId();
     
        if (studentId is null)
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
