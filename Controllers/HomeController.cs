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
    var today = DateTime.Now.Date;

    var model = new DashboardViewModel
    {
        TotalStationeries = await _context.Stationeries.IgnoreQueryFilters().CountAsync(),
        TotalOrders = await _context.Orders.CountAsync(),
        TotalAuditLogs = await _context.AuditLogs.CountAsync(),
        SecurityControlsEnabled = 8,

        AccessDeniedToday = await _context.AuditLogs
            .AsNoTracking()
            .CountAsync(l => l.Action == "AccessDenied" && l.CreatedAt >= today),

        SensitiveActionsToday = await _context.AuditLogs
            .AsNoTracking()
            .CountAsync(l => l.CreatedAt >= today &&
                (l.Action == "SoftDeleteStationery" || l.Action == "EditStationery" || l.Action == "AdjustStock")),

        UploadRejectedToday = await _context.AuditLogs
            .AsNoTracking()
            .CountAsync(l => (l.Action == "UploadStationeryImage" || l.Action == "ReplaceStationeryImage")
                && l.Result == "Failed" && l.CreatedAt >= today)
    };

    return View(model);
}
    
}