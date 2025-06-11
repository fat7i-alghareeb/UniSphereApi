// Ignore Spelling: app

using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;

namespace UniSphere.Api.Extensions;

public static class DatabaseExtension
{
    public static async Task ApplyMigrationsAsync(this WebApplication app) {
        using IServiceScope scope = app.Services.CreateScope();
        await using ApplicationDbContext dBContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await dBContext.Database.MigrateAsync();
            app.Logger.LogInformation("Database migrations applied successfully.");

        }
        catch (Exception ex) {
            app.Logger.LogError(ex, "An error occurred while applying database migrations.");
            throw;
        }
             
    }
}
