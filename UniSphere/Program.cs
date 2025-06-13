    using UniSphere.Api;
using UniSphere.Api.Extensions;
using UniSphere.Api.Database.Seeding;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.AddControllers()
    .AddErrorHandling()
    .AddDatabase()
    .AddObservability()
    .AddApplicationServices();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "UniSphere API V1");


    });
    await app.ApplyMigrationsAsync();
    using IServiceScope scope = app.Services.CreateScope();
    DatabaseSeeder seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.ClearAllDataAsync();
    await seeder.SeedAsync();
}
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Logger.LogInformation("Application started successfully.");

await app.RunAsync();
