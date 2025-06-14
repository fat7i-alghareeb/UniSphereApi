using Microsoft.AspNetCore.Identity;

namespace UniSphere.Api.Entities;

public class ApplicationUser : IdentityUser
{
    public Guid? StudentId { get; set; }
} 
