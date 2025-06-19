using UniSphere.Api;
using UniSphere.Api.Extensions;
using UniSphere.Api.Database.Seeding;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.AddControllers()
    .AddErrorHandling()
    .AddDatabase()
    .AddObservability()
    .AddApplicationServices()
    .AddAuthenticationServices();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "UniSphere API V1");
    });
    await app.ApplyMigrationsAsync();
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.ClearIdentityDataAsync();
    await seeder.ClearApplicationDataAsync();
    await seeder.SeedRolesAsync();
    await seeder.SeedAsync();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    app.Urls.Clear();
    app.Urls.Add($"http://0.0.0.0:{port}");
}

app.Logger.LogInformation("Application started successfully.");

await app.RunAsync();



// On Railway
// In the Railway dashboard, go to your project → Variables.
// Add a variable:
// Key: ConnectionStrings__Database
// Value: (your Railway Postgres connection string, e.g. postgresql://user:password@host:port/dbname)
// > The double underscore __ is how .NET maps environment variables to nested config sections.
// > So ConnectionStrings__Database becomes ConnectionStrings:Database in .NET.