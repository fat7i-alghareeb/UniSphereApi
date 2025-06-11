using UniSphere.Api;
using UniSphere.Api.Extensions;

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
}

app.UseHttpsRedirection();
app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();


app.Logger.LogInformation("Application started successfully.");

await app.RunAsync();
