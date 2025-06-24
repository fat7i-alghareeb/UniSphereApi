using UniSphere.Api.DTOs.Auth;
using UniSphere.Api.Entities;

namespace UniSphere.Api.Services;

public interface IAuthService
{
    Task<RefreshToken> CreateOrUpdateRefreshTokenAsync(ApplicationUser user, string refreshToken);
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task<bool> ValidateOneTimeCodeAsync(StudentCredential studentCredential, int code);
    Task<bool> ValidateOneTimeCodeAsync(Admin admin, int code);
    Task<bool> ValidateOneTimeCodeAsync(SuperAdmin superAdmin, int code);
    Task<bool> ValidateOneTimeCodeAsync(Professor professor, int code);
} 