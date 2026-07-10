using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniStationery.Mvc.Data;

namespace MiniStationery.Mvc.Controllers;

public class ApiDemoController : Controller
{
    private readonly ApplicationDbContext _context;

    public ApiDemoController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> StationeryError(int id = 9999)
    {
        var item = await _context.Stationeries.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);

        object result = item != null
            ? item
            : new
            {
                type = "https://example.com/problems/stationery-not-found",
                title = "Stationery not found",
                status = 404,
                detail = $"Không tìm thấy mặt hàng với Id = {id}.",
                instance = $"/api/stationery/{id}",
                traceId = HttpContext.TraceIdentifier
            };

        ViewBag.Json = System.Text.Json.JsonSerializer.Serialize(result, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        return View();
    }
}