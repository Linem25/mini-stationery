using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniStationery.Mvc.Data;
using MiniStationery.Mvc.ViewModels;

namespace MiniStationery.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            TotalStationeries = await _context.Stationeries.IgnoreQueryFilters().CountAsync(),
            TotalOrders = await _context.Orders.CountAsync(),
            TotalAuditLogs = await _context.AuditLogs.CountAsync(),
            SecurityControlsEnabled = 8 
        };

        return View(model);
    }
}