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
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
    await app.ApplyMigrationsAsync();
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    //await seeder.ClearIdentityDataAsync();
   // await seeder.ClearApplicationDataAsync();
    await seeder.SeedRolesAsync();
    await seeder.SeedAsync();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

// Enable static file serving from wwwroot
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Logger.LogInformation("Application started successfully.");

await app.RunAsync();
