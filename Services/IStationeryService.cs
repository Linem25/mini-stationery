using MiniStationery.Mvc.ViewModels;

namespace MiniStationery.Mvc.Services;

public interface IStationeryService
{
    Task<List<StationeryListItemViewModel>> GetStationeryListAsync();
    Task<StationeryDetailViewModel?> GetByIdAsync(int id);
    Task<List<StationeryListItemViewModel>> GetLowStockAsync();
    Task<StationeryFilterViewModel> FilterAsync(int? categoryId, decimal? minPrice, decimal? maxPrice);
}