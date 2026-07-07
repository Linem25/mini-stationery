using Microsoft.Extensions.Options;
using MiniStationery.Mvc.Options;
using MiniStationery.Mvc.Repositories;
using MiniStationery.Mvc.ViewModels;

namespace MiniStationery.Mvc.Services;

public class StationeryService : IStationeryService
{
    private readonly IStationeryRepository _stationeryRepository;
    private readonly AppSettings _settings;

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
}