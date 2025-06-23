using Microsoft.AspNetCore.Identity;

namespace UniSphere.Api.Entities;

public class ApplicationUser : IdentityUser
{
    public Guid? StudentId { get; set; }
    public Guid? AdminId { get; set; }
    public Guid? SuperAdminId { get; set; }
    public Guid? ProfessorId { get; set; }
} 
