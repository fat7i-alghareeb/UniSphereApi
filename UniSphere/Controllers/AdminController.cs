using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.Entities;
using UniSphere.Api.Extensions;

namespace UniSphere.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Authorize]
[Route("api/[controller]")]
public class AdminController(ApplicationDbContext dbContext) : BaseController
{
    [HttpGet("GetMe")]
    public async Task<ActionResult<BaseAdminDto>> GetMe()
    {
        var adminId = HttpContext.User.GetAdminId();

        if (adminId is null)
        {
            return Unauthorized();
        }

        Admin admin = await dbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Id == adminId);
        if (admin is null)
        {
            return Unauthorized();
        }
        return Ok(admin.ToBaseAdminDto());
    }
} 