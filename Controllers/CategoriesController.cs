using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniStationery.Mvc.Data;

namespace MiniStationery.Mvc.Controllers;

public class CategoriesController : Controller
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _context.Categories
            .Include(c => c.Stationeries)
            .AsNoTracking()
            .Select(c => new
            {
                c.Id,
                c.Name,
                TotalItems = c.Stationeries.Count
            })
            .ToListAsync();

        return View(categories);
    }
}