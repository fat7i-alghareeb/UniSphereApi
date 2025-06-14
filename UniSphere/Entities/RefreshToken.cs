using Microsoft.AspNetCore.Identity;

namespace UniSphere.Api.Entities;

public sealed class RefreshToken
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAtUtc { get; set; }
    public string UserId { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAtUtc;

    public ApplicationUser User { get; set; }
}
