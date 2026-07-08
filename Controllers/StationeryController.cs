using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniStationery.Mvc.Data;
using MiniStationery.Mvc.Services;
using MiniStationery.Mvc.ViewModels;

namespace MiniStationery.Mvc.Controllers;

public class StationeryController : Controller
{
    private readonly IStationeryService _stationeryService;
    private readonly AppDbContext _context;
    private readonly ILogger<StationeryController> _logger;

    public StationeryController(
        IStationeryService stationeryService,
        AppDbContext context,
        ILogger<StationeryController> logger)
    {
        _stationeryService = stationeryService;
        _context = context;
        _logger = logger;
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

    [HttpGet]
    public IActionResult Create()
    {
        return View(new StationeryCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StationeryCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _stationeryService.CreateAsync(model);
            TempData["Success"] = "Đã thêm mặt hàng thành công.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(nameof(model.SupplyCode), ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var detail = await _stationeryService.GetByIdAsync(id);
        if (detail == null) return NotFound();

        var model = new StationeryEditViewModel
        {
            Id = detail.Id,
            Name = detail.Name,
            Price = detail.UnitPrice,
            Stock = detail.Quantity
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StationeryEditViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);

        var stationery = await _context.Stationeries.FirstOrDefaultAsync(s => s.Id == id);
        if (stationery == null) return NotFound();

        stationery.Name = model.Name;
        stationery.SupplyCode = model.SupplyCode;
        stationery.Price = model.Price;
        stationery.Stock = model.Stock;
        stationery.CategoryId = model.CategoryId;
        stationery.Description = model.Description;
        stationery.UpdatedAt = DateTime.Now;

        _context.Entry(stationery).Property("RowVersion").OriginalValue =
            Convert.FromBase64String(model.RowVersion);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Stationery updated. Id={Id}", id);
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(string.Empty,
                "Dữ liệu đã được người khác cập nhật. Vui lòng tải lại trang và thử lại.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _stationeryService.SoftDeleteAsync(id);
        if (!success) return NotFound();

        TempData["Success"] = "Đã xóa mềm mặt hàng.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Trash()
    {
        var deletedItems = await _stationeryService.GetTrashAsync();
        return View(deletedItems);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(int id)
    {
        var success = await _stationeryService.RestoreAsync(id);
        if (!success) return NotFound();

        return RedirectToAction(nameof(Trash));
    }
}