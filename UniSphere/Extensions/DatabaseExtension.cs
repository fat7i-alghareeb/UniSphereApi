// Ignore Spelling: app

using Microsoft.EntityFrameworkCore;
using UniSphere.Api.Database;

namespace UniSphere.Api.Extensions;

public static class DatabaseExtension
{
    public static async Task ApplyMigrationsAsync(this WebApplication app) {
        using IServiceScope scope = app.Services.CreateScope();
        await using ApplicationDBContext  dBContext =scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();

        try
        {
            await dBContext.Database.MigrateAsync();
            app.Logger.LogInformation("Database migrations applied successfully.");

        }
        catch (Exception ex) {
            app.Logger.LogError(ex, "An Error occured while applying database migrations.");
            throw;
        }
             
    }
}
