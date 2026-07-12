using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniStationery.Mvc.Data;
using MiniStationery.Mvc.Services;
using MiniStationery.Mvc.ViewModels;

namespace MiniStationery.Mvc.Controllers;

[Authorize(Policy = "CanViewStationery")]
public class StationeryController : Controller
{
    private readonly IStationeryService _stationeryService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<StationeryController> _logger;
    private readonly IAuditLogService _auditLogService;
    private readonly IFileUploadService _fileUploadService;

    public StationeryController(
        IStationeryService stationeryService,
        ApplicationDbContext context,
        ILogger<StationeryController> logger,
        IAuditLogService auditLogService,
        IFileUploadService fileUploadService)
    {
        _stationeryService = stationeryService;
        _context = context;
        _logger = logger;
        _auditLogService = auditLogService;
        _fileUploadService = fileUploadService;
    }

    // ================= INDEX =================
    public async Task<IActionResult> Index()
    {
        var items = await _stationeryService.GetStationeryListAsync();
        return View(items);
    }

    // ================= DETAIL =================
    public async Task<IActionResult> Detail(int id)
    {
        var item = await _stationeryService.GetByIdAsync(id);

        if (item == null)
        {
            return NotFound($"Không tìm thấy mặt hàng có id = {id}");
        }

        return View(item);
    }

    // ================= CREATE =================
    [Authorize(Policy = "CanManageStationery")]
    [HttpGet]
    public IActionResult Create()
    {
        return View(new StationeryCreateViewModel());
    }

    [Authorize(Policy = "CanManageStationery")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StationeryCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            await _stationeryService.CreateAsync(model);
            await _auditLogService.LogAsync("CreateStationery", "Stationery", model.SupplyCode, "Success");
            TempData["Success"] = "Đã thêm mặt hàng thành công.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            await _auditLogService.LogAsync("CreateStationery", "Stationery", model.SupplyCode, "Failed", ex.Message);
            ModelState.AddModelError(nameof(model.SupplyCode), ex.Message);
            return View(model);
        }
    }

    // ================= EDIT (Feature 2: Safe Image Replace) =================
    [Authorize(Policy = "CanManageStationery")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var stationery = await _context.Stationeries.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        if (stationery == null) return NotFound();

        var model = new StationeryEditViewModel
        {
            Id = stationery.Id,
            Name = stationery.Name,
            SupplyCode = stationery.SupplyCode,
            Price = stationery.Price,
            Stock = stationery.Stock,
            CategoryId = stationery.CategoryId,
            Description = stationery.Description,
            ExistingImageUrl = stationery.ImageUrl,
            RowVersion = Convert.ToBase64String(stationery.RowVersion)
        };

        return View(model);
    }

    [Authorize(Policy = "CanManageStationery")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StationeryEditViewModel model)
    {
        if (id != model.Id) return NotFound();
        if (!ModelState.IsValid) return View(model);

        var stationery = await _context.Stationeries.FirstOrDefaultAsync(s => s.Id == id);
        if (stationery == null) return NotFound();

        var oldImageUrl = stationery.ImageUrl;
        string? newImageUrl = null;

        if (model.ImageFile != null && model.ImageFile.Length > 0)
        {
            try
            {
                newImageUrl = await _fileUploadService.SaveStationeryImageAsync(model.ImageFile);
            }
            catch (InvalidOperationException ex)
            {
                await _auditLogService.LogAsync("ReplaceStationeryImage", "Stationery", id.ToString(), "Failed", ex.Message);
                ModelState.AddModelError(nameof(model.ImageFile), ex.Message);
                model.ExistingImageUrl = oldImageUrl;
                model.RowVersion = Convert.ToBase64String(stationery.RowVersion);
                return View(model);
            }
        }

        stationery.Name = model.Name;
        stationery.SupplyCode = model.SupplyCode;
        stationery.Price = model.Price;
        stationery.Stock = model.Stock;
        stationery.CategoryId = model.CategoryId;
        stationery.Description = model.Description;
        stationery.UpdatedAt = DateTime.Now;

        if (newImageUrl != null)
        {
            stationery.ImageUrl = newImageUrl;
        }

        _context.Entry(stationery).Property("RowVersion").OriginalValue =
            Convert.FromBase64String(model.RowVersion);

        try
        {
            await _context.SaveChangesAsync();

            if (newImageUrl != null && !string.IsNullOrEmpty(oldImageUrl))
            {
                _fileUploadService.DeleteStationeryImage(oldImageUrl);
                await _auditLogService.LogAsync("ReplaceStationeryImage", "Stationery", id.ToString(), "Success");
            }

            await _auditLogService.LogAsync("EditStationery", "Stationery", id.ToString(), "Success");
            _logger.LogInformation("Stationery updated. Id={Id}", id);
            TempData["Success"] = "Đã cập nhật mặt hàng thành công.";
            return RedirectToAction(nameof(Detail), new { id });
        }
        catch (DbUpdateConcurrencyException)
        {
            await _auditLogService.LogAsync("EditStationery", "Stationery", id.ToString(), "Failed", "Concurrency conflict");
            ModelState.AddModelError(string.Empty,
                "Dữ liệu đã được người khác cập nhật. Vui lòng tải lại trang và thử lại.");
            model.ExistingImageUrl = oldImageUrl;
            model.RowVersion = Convert.ToBase64String(stationery.RowVersion);
            return View(model);
        }
    }

    // ================= UPLOAD IMAGE (route riêng, giữ tương thích ngược) =================
    [Authorize(Policy = "CanUploadStationeryImage")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadImage(int id, IFormFile imageFile)
    {
        var stationery = await _context.Stationeries.FirstOrDefaultAsync(s => s.Id == id);
        if (stationery == null) return NotFound();

        if (imageFile == null || imageFile.Length == 0)
        {
            TempData["Error"] = "Vui lòng chọn file ảnh.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        var oldImageUrl = stationery.ImageUrl;

        try
        {
            var imagePath = await _fileUploadService.SaveStationeryImageAsync(imageFile);
            stationery.ImageUrl = imagePath;
            stationery.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            if (!string.IsNullOrEmpty(oldImageUrl))
            {
                _fileUploadService.DeleteStationeryImage(oldImageUrl);
            }

            await _auditLogService.LogAsync("ReplaceStationeryImage", "Stationery", id.ToString(), "Success");
            TempData["Success"] = "Đã tải ảnh lên thành công.";
        }
        catch (InvalidOperationException ex)
        {
            await _auditLogService.LogAsync("ReplaceStationeryImage", "Stationery", id.ToString(), "Failed", ex.Message);
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Detail), new { id });
    }

    // ================= DELETE (Soft Delete) =================
    [Authorize(Policy = "CanManageStationery")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _stationeryService.SoftDeleteAsync(id);
        if (!success)
        {
            await _auditLogService.LogAsync("SoftDeleteStationery", "Stationery", id.ToString(), "Failed", "Not found");
            return NotFound();
        }

        await _auditLogService.LogAsync("SoftDeleteStationery", "Stationery", id.ToString(), "Success");

        TempData["Success"] = "Đã xóa mềm mặt hàng.";
        return RedirectToAction(nameof(Index));
    }

    // ================= TRASH & RESTORE =================
    [Authorize(Policy = "CanManageStationery")]
    public async Task<IActionResult> Trash()
    {
        var deletedItems = await _stationeryService.GetTrashAsync();
        return View(deletedItems);
    }

    [Authorize(Policy = "CanManageStationery")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(int id)
    {
        var success = await _stationeryService.RestoreAsync(id);
        if (!success) return NotFound();

        await _auditLogService.LogAsync("RestoreStationery", "Stationery", id.ToString(), "Success");

        return RedirectToAction(nameof(Trash));
    }

    // ================= SEARCH =================
    [HttpGet]
    public async Task<IActionResult> Search(string? keyword, string? stockStatus)
    {
        var viewModel = await _stationeryService.SearchAsync(keyword, stockStatus);
        return View(viewModel);
    }

    // ================= ADJUST STOCK (Feature 1: Admin + Staff) =================
    [Authorize(Policy = "CanAdjustStock")]
    [HttpGet]
    public async Task<IActionResult> AdjustStock(int id)
    {
        var stationery = await _context.Stationeries.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        if (stationery == null) return NotFound();

        var model = new StationeryAdjustStockViewModel
        {
            Id = stationery.Id,
            Name = stationery.Name,
            CurrentStock = stationery.Stock,
            Adjustment = 0,
            RowVersion = Convert.ToBase64String(stationery.RowVersion)
        };

        return View(model);
    }

    [Authorize(Policy = "CanAdjustStock")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AdjustStock(int id, StationeryAdjustStockViewModel model)
    {
        if (id != model.Id) return NotFound();

        var stationery = await _context.Stationeries.FirstOrDefaultAsync(s => s.Id == id);
        if (stationery == null) return NotFound();

        var newStock = stationery.Stock + model.Adjustment;

        if (newStock < 0)
        {
            await _auditLogService.LogAsync("AdjustStock", "Stationery", id.ToString(), "Failed", "Negative stock rejected");
            ModelState.AddModelError(nameof(model.Adjustment), "Số lượng sau điều chỉnh không được nhỏ hơn 0.");
            model.CurrentStock = stationery.Stock;
            model.RowVersion = Convert.ToBase64String(stationery.RowVersion);
            return View(model);
        }

        stationery.Stock = newStock;
        stationery.UpdatedAt = DateTime.Now;

        _context.Entry(stationery).Property("RowVersion").OriginalValue =
            Convert.FromBase64String(model.RowVersion);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation(
                "Stock adjusted. Id={Id}, Adjustment={Adjustment}, NewStock={NewStock}",
                id, model.Adjustment, newStock);

            await _auditLogService.LogAsync("AdjustStock", "Stationery", id.ToString(), "Success",
                $"Adjustment={model.Adjustment}, NewStock={newStock}");

            TempData["Success"] = $"Đã điều chỉnh tồn kho thành công. Số lượng mới: {newStock}";
            return RedirectToAction(nameof(Detail), new { id });
        }
        catch (DbUpdateConcurrencyException)
        {
            await _auditLogService.LogAsync("AdjustStock", "Stationery", id.ToString(), "Failed", "Concurrency conflict");
            ModelState.AddModelError(string.Empty,
                "Dữ liệu đã được người khác cập nhật. Vui lòng tải lại trang và thử lại.");
            model.CurrentStock = stationery.Stock;
            model.RowVersion = Convert.ToBase64String(stationery.RowVersion);
            return View(model);
        }
    }
}