using Microsoft.AspNetCore.Mvc;
using MiniStationery.Mvc.Services;

namespace MiniStationery.Mvc.Controllers;

public class StationeryController : Controller
{
    private readonly IStationeryService _stationeryService;

    public StationeryController(IStationeryService stationeryService)
    {
        _stationeryService = stationeryService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _stationeryService.GetStationeryListAsync();
        return View(items);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var item = await _stationeryService.GetByIdAsync(id);

        if (item == null)
        {
            return NotFound($"Không tìm thấy mặt hàng có id = {id}");
        }

        return View(item);
    }
    public async Task<IActionResult> LowStock()
{
    var items = await _stationeryService.GetLowStockAsync();
    return View(items);
}
[HttpGet]
public async Task<IActionResult> Filter(int? categoryId, decimal? minPrice, decimal? maxPrice)
{
    var viewModel = await _stationeryService.FilterAsync(categoryId, minPrice, maxPrice);
    return View(viewModel);
}
}