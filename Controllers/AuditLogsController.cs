using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniStationery.Mvc.Data;
using MiniStationery.Mvc.Models;
using MiniStationery.Mvc.ViewModels;

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

    [HttpGet]
public async Task<IActionResult> Search(string? userName, string? actionName, string? result, DateTime? fromDate, DateTime? toDate)
{
    var query = _context.AuditLogs.AsNoTracking().AsQueryable();

    if (!string.IsNullOrWhiteSpace(userName))
    {
        query = query.Where(l => l.UserName != null && l.UserName.Contains(userName));
    }

    if (!string.IsNullOrWhiteSpace(actionName))
    {
        query = query.Where(l => l.Action.Contains(actionName));
    }

    if (!string.IsNullOrWhiteSpace(result))
    {
        query = query.Where(l => l.Result == result);
    }

    if (fromDate.HasValue)
    {
        query = query.Where(l => l.CreatedAt >= fromDate.Value);
    }

    if (toDate.HasValue)
    {
        query = query.Where(l => l.CreatedAt <= toDate.Value.AddDays(1).AddTicks(-1));
    }

    var items = await query.OrderByDescending(l => l.CreatedAt).Take(200).ToListAsync();

    var model = new AuditLogSearchViewModel
    {
        UserName = userName,
        ActionName = actionName,
        Result = result,
        FromDate = fromDate,
        ToDate = toDate,
        Items = items
    };

    return View(model);
}
}