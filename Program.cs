using MiniStationery.Mvc.Data;
using MiniStationery.Mvc.Options;
using MiniStationery.Mvc.Repositories;
using MiniStationery.Mvc.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Microsoft.AspNetCore.Identity;
using MiniStationery.Mvc.Models;

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

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IStationeryRepository, StationeryRepository>();
    builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    builder.Services.AddScoped<IStationeryService, StationeryService>();
    builder.Services.AddScoped<IOrderService, OrderService>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IAuditLogService, AuditLogService>();
    builder.Services.AddScoped<IFileUploadService, FileUploadService>();

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("CanViewStationery", p => p.RequireRole("Admin", "Staff"));
        options.AddPolicy("CanManageStationery", p => p.RequireRole("Admin"));
        options.AddPolicy("CanViewAuditLog", p => p.RequireRole("Admin"));
        options.AddPolicy("CanUploadStationeryImage", p => p.RequireRole("Admin"));
    });

    builder.Services.AddHealthChecks()
        .AddCheck("self",
            () => HealthCheckResult.Healthy("Stationery Store đang chạy bình thường."),
            tags: new[] { "live" })
        .AddDbContextCheck<ApplicationDbContext>("database",
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

    using (var scope = app.Services.CreateScope())
    {
        await DbInitializer.SeedIdentityAsync(scope.ServiceProvider);
    }

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

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapHealthChecks("/api/health/live", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("live")
    });

    app.MapHealthChecks("/api/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready")
    });

    app.MapGet("/api/stationery/{id:int}", async (int id, ApplicationDbContext db, HttpContext http) =>
    {
        var stationery = await db.Stationeries.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        if (stationery == null)
        {
            var problem = new ProblemDetails
            {
                Type = "https://example.com/problems/stationery-not-found",
                Title = "Stationery not found",
                Detail = $"The stationery with id {id} was not found.",
                Status = StatusCodes.Status404NotFound,
                Instance = http.Request.Path
            };
            problem.Extensions["errorCode"] = "STATIONERY_NOT_FOUND";
            problem.Extensions["traceId"] = http.TraceIdentifier;
            problem.Extensions["timestamp"] = DateTimeOffset.UtcNow;

            return Results.Problem(
                type: problem.Type,
                title: problem.Title,
                detail: problem.Detail,
                statusCode: problem.Status,
                instance: problem.Instance,
                extensions: problem.Extensions);
        }

        return Results.Ok(stationery);
    });

    app.MapDefaultControllerRoute();

    Log.Information("Mini Stationery Lab06 đang khởi động...");

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