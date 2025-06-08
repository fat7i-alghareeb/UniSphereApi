using Microsoft.EntityFrameworkCore;

namespace UniSphere.Api.Database;

public sealed class ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : DbContext(options) 
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemes.Application);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDBContext).Assembly);
    }
}
