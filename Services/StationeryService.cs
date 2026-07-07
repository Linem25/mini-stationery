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
}