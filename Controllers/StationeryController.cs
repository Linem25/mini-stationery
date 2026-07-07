using MiniStationery.Mvc.Models;
using MiniStationery.Mvc.Services;
using MiniStationery.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace MiniStationery.Mvc.Controllers;

public class StationeryController : Controller
{
    private readonly StationeryService _stationeryService;

    public StationeryController(StationeryService stationeryService)
    {
        _stationeryService = stationeryService;
    }

    public IActionResult Index()
    {
        var items = _stationeryService.GetAll()
            .Select(ToListItemViewModel)
            .ToList();

        return View(items);
    }

    public IActionResult Detail(int id)
    {
        var item = _stationeryService.GetById(id);

        if (item == null)
        {
            return NotFound($"Không tìm thấy mặt hàng có id = {id}");
        }

        var viewModel = ToDetailViewModel(item);

        return View(viewModel);
    }

    public IActionResult Stats()
    {
        var stats = _stationeryService.GetStats();

        return View(stats);
    }

    public IActionResult Welcome()
    {
        return Content("Welcome to ASP.NET Core MVC Lab02 - Mini Stationery");
    }

    public IActionResult StationeryJson()
    {
        var items = _stationeryService.GetAll()
            .Select(item => new
            {
                item.Id,
                item.Sku,
                item.Name,
                item.Category,
                item.UnitPrice,
                item.Quantity
            });

        return Json(items);
    }

    public IActionResult GoToList()
    {
        return RedirectToAction(nameof(Index));
    }

    public IActionResult Force404()
    {
        return NotFound("Đây là response 404 demo từ action Force404.");
    }

    private static StationeryListItemViewModel ToListItemViewModel(Stationery item)
    {
        return new StationeryListItemViewModel
        {
            Id = item.Id,
            Sku = item.Sku,
            Name = item.Name,
            Category = item.Category,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity,
            MinStock = item.MinStock
        };
    }

    private static StationeryDetailViewModel ToDetailViewModel(Stationery item)
    {
        return new StationeryDetailViewModel
        {
            Id = item.Id,
            Sku = item.Sku,
            Name = item.Name,
            Category = item.Category,
            Supplier = item.Supplier,
            UnitPrice = item.UnitPrice,
            Quantity = item.Quantity,
            MinStock = item.MinStock,
            LastUpdatedAt = item.LastUpdatedAt
        };
    }
    [HttpGet]
public IActionResult Search(string? keyword, decimal? minPrice)
{
    var items = _stationeryService.Search(keyword, minPrice)
        .Select(ToListItemViewModel)
        .ToList();

    var viewModel = new StationerySearchViewModel
    {
        Keyword = keyword ?? "",
        MinPrice = minPrice,
        Items = items
    };

    return View(viewModel);
}

[HttpGet]
public IActionResult Create()
{
    var viewModel = new StationeryCreateViewModel
    {
        Quantity = 1,
        MinStock = 1
    };

    return View(viewModel);
}

[HttpPost]
[ValidateAntiForgeryToken]
public IActionResult Create(StationeryCreateViewModel model)
{
    if (!ModelState.IsValid)
    {
        return View(model);
    }

    _stationeryService.Create(model);

    TempData["SuccessMessage"] = "Đã thêm mặt hàng thành công.";

    return RedirectToAction(nameof(Index));
}
}