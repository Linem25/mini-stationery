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
}