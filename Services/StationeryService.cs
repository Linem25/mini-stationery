using Microsoft.Extensions.Options;
using MiniStationery.Mvc.Options;
using MiniStationery.Mvc.Repositories;
using MiniStationery.Mvc.ViewModels;
using Microsoft.EntityFrameworkCore;
using MiniStationery.Mvc.Data;

namespace MiniStationery.Mvc.Services;

public class StationeryService : IStationeryService
{
    private readonly IStationeryRepository _stationeryRepository;
    private readonly AppSettings _settings;
    private readonly AppDbContext _context;
    private readonly ILogger<StationeryService> _logger;

    public StationeryService(IStationeryRepository stationeryRepository, IOptions<AppSettings> options)
    {
        _stationeryRepository = stationeryRepository;
        _settings = options.Value;
    }

    public async Task<List<StationeryListItemViewModel>> GetStationeryListAsync()
    {
        var items = await _stationeryRepository.GetAllReadOnlyAsync();

        return items.Select(s => new StationeryListItemViewModel
        {
            Id = s.Id,
            SupplyCode = s.SupplyCode,
            Name = s.Name,
            UnitPrice = s.Price,
            Quantity = s.Stock,
            Category = s.Category != null ? s.Category.Name : "N/A"
        }).ToList();
    }

    public async Task<StationeryDetailViewModel?> GetByIdAsync(int id)
    {
        var stationery = await _stationeryRepository.GetByIdAsync(id);
        if (stationery == null) return null;

        return new StationeryDetailViewModel
        {
            Id = stationery.Id,
            Sku = "",
            Name = stationery.Name,
            Category = stationery.Category != null ? stationery.Category.Name : "N/A",
            Supplier = "",
            UnitPrice = stationery.Price,
            Quantity = stationery.Stock,
            MinStock = 0,
            LastUpdatedAt = DateTime.Now
        };
    }

    public async Task<List<StationeryListItemViewModel>> GetLowStockAsync()
    {
        var items = await _stationeryRepository.GetAllReadOnlyAsync();

        return items
            .Where(s => s.Stock <= _settings.LowStockThreshold)
            .Select(s => new StationeryListItemViewModel
            {
                Id = s.Id,
                Name = s.Name,
                UnitPrice = s.Price,
                Quantity = s.Stock,
                Category = s.Category != null ? s.Category.Name : "N/A"
            })
            .ToList();
    }

    public async Task<StationeryFilterViewModel> FilterAsync(int? categoryId, decimal? minPrice, decimal? maxPrice)
    {
        var filtered = await _stationeryRepository.FilterAsync(categoryId, minPrice, maxPrice);

        var items = filtered.Select(s => new StationeryListItemViewModel
        {
            Id = s.Id,
            Name = s.Name,
            UnitPrice = s.Price,
            Quantity = s.Stock,
            Category = s.Category != null ? s.Category.Name : "N/A"
        }).ToList();

        var allItems = await _stationeryRepository.GetAllReadOnlyAsync();
        var categories = allItems
            .Where(s => s.Category != null)
            .Select(s => new CategoryOptionViewModel { Id = s.Category!.Id, Name = s.Category.Name })
            .DistinctBy(c => c.Id)
            .ToList();

        return new StationeryFilterViewModel
        {
            CategoryId = categoryId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Items = items,
            Categories = categories
        };
    }

    public StationeryService(
    IStationeryRepository stationeryRepository,
    IOptions<AppSettings> options,
    AppDbContext context,
    ILogger<StationeryService> logger)
{
    _stationeryRepository = stationeryRepository;
    _settings = options.Value;
    _context = context;
    _logger = logger;
}

public async Task CreateAsync(StationeryCreateViewModel model)
{
    var exists = await _context.Stationeries
        .IgnoreQueryFilters()
        .AnyAsync(s => s.SupplyCode == model.SupplyCode);

    if (exists)
    {
        throw new InvalidOperationException("Mã hàng này đã tồn tại.");
    }

    var stationery = new Models.Stationery
    {
        Name = model.Name,
        SupplyCode = model.SupplyCode,
        Price = model.Price,
        Stock = model.Stock,
        CategoryId = model.CategoryId,
        Description = model.Description,
        CreatedAt = DateTime.Now
    };

    _context.Stationeries.Add(stationery);
    await _context.SaveChangesAsync();
    _logger.LogInformation("Stationery created. Id={Id}, SupplyCode={SupplyCode}", stationery.Id, stationery.SupplyCode);
}

public async Task<bool> SoftDeleteAsync(int id)
{
    var stationery = await _context.Stationeries.FirstOrDefaultAsync(s => s.Id == id);
    if (stationery == null) return false;

    stationery.IsDeleted = true;
    stationery.DeletedAt = DateTime.Now;
    stationery.UpdatedAt = DateTime.Now;

    await _context.SaveChangesAsync();
    _logger.LogWarning("Stationery soft deleted. Id={Id}", id);
    return true;
}

public async Task<List<StationeryTrashItemViewModel>> GetTrashAsync()
{
    return await _context.Stationeries
        .IgnoreQueryFilters()
        .Where(s => s.IsDeleted)
        .AsNoTracking()
        .Select(s => new StationeryTrashItemViewModel
        {
            Id = s.Id,
            Name = s.Name,
            DeletedAt = s.DeletedAt
        })
        .ToListAsync();
}

public async Task<bool> RestoreAsync(int id)
{
    var stationery = await _context.Stationeries
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(s => s.Id == id && s.IsDeleted);

    if (stationery == null) return false;

    stationery.IsDeleted = false;
    stationery.DeletedAt = null;
    stationery.UpdatedAt = DateTime.Now;

    await _context.SaveChangesAsync();
    _logger.LogInformation("Stationery restored. Id={Id}", id);
    return true;
}
}