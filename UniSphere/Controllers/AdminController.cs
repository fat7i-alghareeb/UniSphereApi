using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.Entities;
using UniSphere.Api.Extensions;
using UniSphere.Api.Services;

namespace UniSphere.Api.Controllers;

[ApiController]
[Produces("application/json")]
[Route("api/[controller]")]
public class AdminController(
    ApplicationDbContext dbContext,
    UserManager<ApplicationUser> userManager,
    ApplicationIdentityDbContext identityDbContext,
    TokenProvider tokenProvider,
    IAuthService authService
) : BaseController
{
    [HttpGet("GetMe")]
    [Authorize(Roles = "Admin")]
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

    // Authentication Endpoints
    [HttpPost("Auth/CheckOneTimeCode")]
    [AllowAnonymous]
    public async Task<ActionResult<SimpleAdminDto>> CheckOneTimeCode(AdminCheckOneTimeCodeDto checkOneTimeCodeDto)
    {
        var admin = await dbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Gmail == checkOneTimeCodeDto.Gmail && a.MajorId == checkOneTimeCodeDto.MajorId);
        if (admin is null)
        {
            return NotFound();
        }

        if (!await authService.ValidateOneTimeCodeAsync(admin, checkOneTimeCodeDto.Code))
        {
            return Problem(
                detail: "Code is not valid",
                title: "Code is not valid",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        return Ok(admin.ToSimpleAdminDto());
    }

    [HttpPost("Auth/Register")]
    [AllowAnonymous]
    public async Task<ActionResult<FullInfoAdminDto>> Register(RegisterAdminDto registerAdminDto)
    {
        using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        dbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await dbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        Admin? admin = await dbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Id == registerAdminDto.AdminId);
        if (admin is null)
        {
            return NotFound();
        }

        var applicationUser = new ApplicationUser
        {
            UserName = registerAdminDto.AdminId.ToString(),
            AdminId = registerAdminDto.AdminId,
        };
        if (registerAdminDto.Password != registerAdminDto.ConfirmPassword)
        {
            return BadRequest("Password and ConfirmPassword must be the same");
        }

        IdentityResult createAdminResult = await userManager.CreateAsync(applicationUser, registerAdminDto.Password);
        if (!createAdminResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createAdminResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating Admin",
                title: "Error creating Admin",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        IdentityResult addRoleResult = await userManager.AddToRoleAsync(applicationUser, Roles.Admin);
        if (!addRoleResult.Succeeded)
        {
            var extensions = new Dictionary<string, object?>
            {
                { "errors", createAdminResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };

            return Problem(
                detail: "Error creating Admin",
                title: "Error creating Admin",
                statusCode: StatusCodes.Status400BadRequest,
                extensions: extensions
            );
        }
        admin.IdentityId = applicationUser.Id;
        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest([Roles.Admin], null, registerAdminDto.AdminId)
        );
        await identityDbContext.SaveChangesAsync();
        return Ok(admin.ToFullInfoAdminDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }

    [HttpPost("Auth/Login")]
    [AllowAnonymous]
    public async Task<ActionResult<FullInfoAdminDto>> Login(LoginAdminDto loginAdminDto)
    {
        Admin? admin = await dbContext.Admins
            .Include(a => a.Major)
            .FirstOrDefaultAsync(a => a.Gmail == loginAdminDto.Gmail && a.MajorId == loginAdminDto.MajorId);
        if (admin is null)
        {
            return Problem(
                detail: "Admin not found",
                title: "Admin not found",
                statusCode: StatusCodes.Status404NotFound
            );
        }

        ApplicationUser? applicationUser =
            await userManager.Users.FirstOrDefaultAsync(u => u.AdminId == admin.Id);
        if (applicationUser is null)
        {
            return Problem(
                detail: "Admin not Registered",
                title: "Admin not Registered",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        if (!await userManager.CheckPasswordAsync(applicationUser, loginAdminDto.Password))
        {
            return Problem(
                detail: "Wrong password",
                title: "Wrong password",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        IList<string> roles = await userManager.GetRolesAsync(applicationUser);
        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(roles, null, admin.Id)
        );
        
        await authService.CreateOrUpdateRefreshTokenAsync(applicationUser, accessTokens.RefreshToken);
        
        return Ok(admin.ToFullInfoAdminDto(accessTokens.AccessToken, accessTokens.RefreshToken));
    }
} 