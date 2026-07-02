using MiniStationery.Api.Models;
using MiniStationery.Api.Services;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Read config and environment
Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"Application: {builder.Environment.ApplicationName}");

// Register services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<StationeryService>();
builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();

// Minimal API endpoints
app.MapGet("/", () => Results.Ok(new { message = "Mini Stationery API is running" }));

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapGet("/env", () => Results.Ok(new
{
    Environment = app.Environment.EnvironmentName
}));

app.MapGet("/config", (IConfiguration config) => Results.Ok(new
{
    AppName = config["AppSettings:AppName"],
    BaseUrl = config["AppSettings:BaseUrl"]
}));

// Controller endpoints
app.MapControllers();

app.Run();