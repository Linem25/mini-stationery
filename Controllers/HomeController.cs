using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniStationery.Mvc.Data;
using MiniStationery.Mvc.ViewModels;

namespace MiniStationery.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;

    public HomeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel
        {
            TotalStationeries = await _context.Stationeries.IgnoreQueryFilters().CountAsync(),
            ActiveStationeries = await _context.Stationeries.CountAsync(),
            DeletedStationeries = await _context.Stationeries.IgnoreQueryFilters().CountAsync(s => s.IsDeleted),
            LogsToday = CountLogLinesToday()
        };

        return View(model);
    }

    private int CountLogLinesToday()
    {
        var path = $"logs/lab05-{DateTime.Now:yyyyMMdd}.txt";

        if (!System.IO.File.Exists(path))
        {
            return 0;
        }

        try
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = new StreamReader(stream);
            var count = 0;
            while (reader.ReadLine() != null)
            {
                count++;
            }
            return count;
        }
        catch
        {
            return 0;
        }
    }
}