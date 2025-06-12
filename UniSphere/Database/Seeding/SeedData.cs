namespace UniSphere.Api.Database.Seeding;

public abstract class SeedData(ApplicationDbContext context)
{
    protected readonly ApplicationDbContext Context = context;

    public abstract Task SeedAsync();
}
