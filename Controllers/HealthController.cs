using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MiniStationery.Mvc.ViewModels;

namespace MiniStationery.Mvc.Controllers;

public class HealthController : Controller
{
    private readonly HealthCheckService _healthCheckService;

    public HealthController(HealthCheckService healthCheckService)
    {
        _healthCheckService = healthCheckService;
    }

    public async Task<IActionResult> Ready()
    {
        var report = await _healthCheckService.CheckHealthAsync();

        var model = new HealthCheckViewModel
        {
            PageTitle = "Health Check - /health/ready",
            OverallStatus = report.Status.ToString(),
            Checks = report.Entries.Select(e => new HealthCheckItemViewModel
            {
                Name = e.Key,
                Status = e.Value.Status.ToString(),
                Description = e.Value.Description ?? ""
            }).ToList()
        };

        return View("Index", model);
    }

    [HttpGet]
[Route("/api/health/ready")]
public async Task<IActionResult> ReadyJson()
{
    var report = await _healthCheckService.CheckHealthAsync();

    var result = new
    {
        status = report.Status.ToString(),
        totalDuration = report.TotalDuration.TotalMilliseconds,
        checks = report.Entries.Select(e => new
        {
            name = e.Key,
            status = e.Value.Status.ToString(),
            description = e.Value.Description
        })
    };

    return Json(result);
}
}