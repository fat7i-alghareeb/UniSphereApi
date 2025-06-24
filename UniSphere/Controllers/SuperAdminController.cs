using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.Entities;
using UniSphere.Api.Extensions;

namespace UniSphere.Api.Controllers;

[ApiController]
[Authorize(Roles = "SuperAdmin")]
[Route("api/[controller]")]
public class SuperAdminController(ApplicationDbContext dbContext) : BaseController
{
    [HttpPost("AssignOneTimeCode")]
    public async Task<IActionResult> AssignOneTimeCode([FromBody] AssignOneTimeCodeRequestDto dto)
    {
        var superAdminId = HttpContext.User.GetSuperAdminId();
        if (superAdminId is null)
        {
            return Unauthorized();
        }
        var superAdmin = await dbContext.SuperAdmins.FirstOrDefaultAsync(sa => sa.Id == superAdminId);
        if (superAdmin is null)
        {
            return Unauthorized();
        }
        var facultyId = superAdmin.FacultyId;
        int code = Random.Shared.Next(100_000, 1_000_000); // 6-digit code
        int expiration = dto.ExpirationInMinutes ?? 10;
        DateTime now = DateTime.UtcNow;
        switch (dto.TargetRole)
        {
            case AssignOneTimeCodeTargetRole.Admin:
                if (dto.AdminId is null){
                    return BadRequest("AdminId is required.");
                }
                var admin = await dbContext.Admins.Include(a => a.Major).FirstOrDefaultAsync(a => a.Id == dto.AdminId);
                if (admin is null || admin.Major.FacultyId != facultyId){
                    return Forbid();
                }
                admin.OneTimeCode = code;
                admin.OneTimeCodeCreatedDate = now;
                admin.OneTimeCodeExpirationInMinutes = expiration;
                break;
            case AssignOneTimeCodeTargetRole.Professor:
                if (dto.ProfessorId is null){
                    return BadRequest("ProfessorId is required.");
                }
                var professorLink = await dbContext.ProfessorFacultyLinks.FirstOrDefaultAsync(pfl => pfl.ProfessorId == dto.ProfessorId && pfl.FacultyId == facultyId);
                if (professorLink is null){
                    return Forbid();
                }
                var professor = await dbContext.Professors.FirstOrDefaultAsync(p => p.Id == dto.ProfessorId);
                if (professor is null){
                    return NotFound("Professor not found.");
                }
                professor.OneTimeCode = code;
                professor.OneTimeCodeCreatedDate = now;
                professor.OneTimeCodeExpirationInMinutes = expiration;
                break;
            case AssignOneTimeCodeTargetRole.Student:
                    if (dto.StudentId is null){
                    return BadRequest("StudentId is required.");
                }
                var student = await dbContext.StudentCredentials.Include(sc => sc.Major).FirstOrDefaultAsync(sc => sc.Id == dto.StudentId);
                if (student is null || student.Major.FacultyId != facultyId){
                    return Forbid();
                }
                student.OneTimeCode = code;
                student.OneTimeCodeCreatedDate = now;
                student.OneTimeCodeExpirationInMinutes = expiration;
                break;
            default:
                return BadRequest("Invalid target role.");
        }
        await dbContext.SaveChangesAsync();
        return Ok(new
        {
            message = "One-time code assigned successfully.",
            code,
            expiresAt = now.AddMinutes(expiration)
        });
    }
} 