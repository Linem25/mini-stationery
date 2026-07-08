using MiniStationery.Mvc.Data;
using MiniStationery.Mvc.Options;
using MiniStationery.Mvc.Repositories;
using MiniStationery.Mvc.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

// Tạo logger sớm trước builder
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/stationery-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddControllersWithViews();

    builder.Services.Configure<AppSettings>(
        builder.Configuration.GetSection("AppSettings"));

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IStationeryRepository, StationeryRepository>();
    builder.Services.AddScoped<IOrderRepository, OrderRepository>();

    builder.Services.AddScoped<IStationeryService, StationeryService>();
    builder.Services.AddScoped<IOrderService, OrderService>();

    builder.Services.AddHealthChecks()
        .AddCheck("self",
            () => HealthCheckResult.Healthy("Stationery Store đang chạy bình thường."),
            tags: new[] { "live" })
        .AddDbContextCheck<AppDbContext>("database",
            tags: new[] { "ready" });

    builder.Services.AddProblemDetails(options =>
    {
        options.CustomizeProblemDetails = context =>
        {
            context.ProblemDetails.Extensions["traceId"] =
                context.HttpContext.TraceIdentifier;
            context.ProblemDetails.Extensions["timestamp"] =
                DateTimeOffset.UtcNow;
        };
    });

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthorization();

    app.MapHealthChecks("/api/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.MapHealthChecks("/api/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

    app.MapGet("/api/stationery/{id:int}", async (
        int id,
        AppDbContext db,
        HttpContext http) =>
    {
        var item = await db.Stationeries
            .AsNoTracking()
            .Include(s => s.Category)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (item == null)
        {
            return Results.Problem(
                type: "https://example.com/problems/stationery-not-found",
                title: "Stationery not found",
                detail: $"Không tìm thấy mặt hàng với Id = {id}.",
                statusCode: StatusCodes.Status404NotFound,
                instance: http.Request.Path);
        }

        return Results.Ok(item);
    });

    app.MapDefaultControllerRoute();

    Log.Information("Mini Stationery Lab05 đang khởi động...");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "App khởi động thất bại.");
}
finally
{
    Log.CloseAndFlush();
}