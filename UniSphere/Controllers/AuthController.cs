using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.Entities;
using UniSphere.Api.Services;
using UniSphere.Api.Helpers;

namespace UniSphere.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public sealed class AuthController(
    UserManager<ApplicationUser> userManager,
    TokenProvider tokenProvider,
    IAuthService authService
) : BaseController
{
    [HttpPost("RefreshToken")]
    public async Task<ActionResult<AccessTokensDto>> RefreshToken(RefreshTokenDto refreshTokenDto)
    {
        RefreshToken? refreshToken = await authService.GetRefreshTokenAsync(refreshTokenDto.RefreshToken);
        if (refreshToken is null)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetUnauthorizedMessage(Lang) });
        }
        if (refreshToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            return Unauthorized(new { message = BilingualErrorMessages.GetTokenExpiredMessage(Lang) });
        }
        IList<string> roles = await userManager.GetRolesAsync(refreshToken.User);

        AccessTokensDto accessTokens = tokenProvider.Create(
            new TokenRequest(roles, refreshToken.User.StudentId, refreshToken.User.AdminId, refreshToken.User.SuperAdminId, refreshToken.User.ProfessorId, refreshToken.User.SystemControllerId)
        );
        
        await authService.CreateOrUpdateRefreshTokenAsync(refreshToken.User, accessTokens.RefreshToken);
      
        return Ok(accessTokens);
    }
} 