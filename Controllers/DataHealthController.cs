using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiniStationery.Mvc.Data;
using MiniStationery.Mvc.Options;
using MiniStationery.Mvc.ViewModels;

namespace MiniStationery.Mvc.Controllers;

public class DataHealthController : Controller
{
    private readonly AppDbContext _context;
    private readonly AppSettings _settings;

    public DataHealthController(AppDbContext context, IOptions<AppSettings> options)
    {
        _context = context;
        _settings = options.Value;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DataHealthViewModel
        {
            SeedDataEnabled = _settings.EnableSeedData,
            TotalCategories = await _context.Categories.CountAsync(),
            TotalStationeries = await _context.Stationeries.CountAsync(),
            TotalOrders = await _context.Orders.CountAsync(),
            DatabaseProvider = _context.Database.ProviderName ?? "Unknown"
        };

        return View(model);
    }
}