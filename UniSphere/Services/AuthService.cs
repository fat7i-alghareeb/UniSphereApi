using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UniSphere.Api.Database;
using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.Entities;
using UniSphere.Api.Settings;

namespace UniSphere.Api.Services;

public class AuthService(
    ApplicationIdentityDbContext identityDbContext,
    IOptions<JwtAuthOptions> jwtAuthOptions
) : IAuthService
{
    private readonly JwtAuthOptions _jwtAuthOptions = jwtAuthOptions.Value;

    public async Task<RefreshToken> CreateOrUpdateRefreshTokenAsync(ApplicationUser user, string refreshToken)
    {
        RefreshToken? existingRefreshToken = await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.UserId == user.Id);

        if (existingRefreshToken is null)
        {
            var newRefreshToken = new RefreshToken
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays),
                Token = refreshToken
            };
            await identityDbContext.RefreshTokens.AddAsync(newRefreshToken);
            await identityDbContext.SaveChangesAsync();
            return newRefreshToken;
        }
        else
        {
            existingRefreshToken.Token = refreshToken;
            existingRefreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays);
            await identityDbContext.SaveChangesAsync();
            return existingRefreshToken;
        }
    }

    public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        return await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public Task<bool> ValidateOneTimeCodeAsync(StudentCredential studentCredential, int code)
    {
        if (studentCredential.OneTimeCodeCreatedDate is null ||
            studentCredential.OneTimeCodeExpirationInMinutes is null || 
            studentCredential.OneTimeCode is null)
        {
            return Task.FromResult(false);
        }

        if (studentCredential.OneTimeCode != code)
        {
            return Task.FromResult(false);
        }

        if (studentCredential.OneTimeCodeCreatedDate!.Value.AddMinutes(
                studentCredential.OneTimeCodeExpirationInMinutes!.Value) < DateTime.UtcNow)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public Task<bool> ValidateOneTimeCodeAsync(Admin admin, int code)
    {
        if (admin.OneTimeCodeCreatedDate is null ||
            admin.OneTimeCodeExpirationInMinutes is null || 
            admin.OneTimeCode is null)
        {
            return Task.FromResult(false);
        }

        if (admin.OneTimeCode != code)
        {
            return Task.FromResult(false);
        }

        if (admin.OneTimeCodeCreatedDate!.Value.AddMinutes(
                admin.OneTimeCodeExpirationInMinutes!.Value) < DateTime.UtcNow)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public Task<bool> ValidateOneTimeCodeAsync(SuperAdmin superAdmin, int code)
    {
        if (superAdmin.OneTimeCodeCreatedDate is null ||
            superAdmin.OneTimeCodeExpirationInMinutes is null || 
            superAdmin.OneTimeCode is null)
        {
            return Task.FromResult(false);
        }

        if (superAdmin.OneTimeCode != code)
        {
            return Task.FromResult(false);
        }

        if (superAdmin.OneTimeCodeCreatedDate!.Value.AddMinutes(
                superAdmin.OneTimeCodeExpirationInMinutes!.Value) < DateTime.UtcNow)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    public Task<bool> ValidateOneTimeCodeAsync(Professor professor, int code)
    {
        if (professor.OneTimeCodeCreatedDate is null ||
            professor.OneTimeCodeExpirationInMinutes is null || 
            professor.OneTimeCode is null)
        {
            return Task.FromResult(false);
        }

        if (professor.OneTimeCode != code)
        {
            return Task.FromResult(false);
        }

        if (professor.OneTimeCodeCreatedDate!.Value.AddMinutes(
                professor.OneTimeCodeExpirationInMinutes!.Value) < DateTime.UtcNow)
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }
} 