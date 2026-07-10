using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniStationery.Mvc.Data;

namespace MiniStationery.Mvc.Controllers;

[Authorize(Policy = "CanViewAuditLog")]
public class AuditLogsController : Controller
{
    private readonly ApplicationDbContext _context;

    public AuditLogsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _context.AuditLogs
            .OrderByDescending(l => l.CreatedAt)
            .Take(100)
            .AsNoTracking()
            .ToListAsync();

        return View(logs);
    }
}