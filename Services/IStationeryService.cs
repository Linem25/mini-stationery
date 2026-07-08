using MiniStationery.Mvc.ViewModels;

namespace MiniStationery.Mvc.Services;

public interface IStationeryService
{
    Task<List<StationeryListItemViewModel>> GetStationeryListAsync();
    Task<StationeryDetailViewModel?> GetByIdAsync(int id);
    Task<List<StationeryListItemViewModel>> GetLowStockAsync();
    Task<StationeryFilterViewModel> FilterAsync(int? categoryId, decimal? minPrice, decimal? maxPrice);
    Task CreateAsync(StationeryCreateViewModel model);
Task<bool> SoftDeleteAsync(int id);
Task<List<StationeryTrashItemViewModel>> GetTrashAsync();
Task<bool> RestoreAsync(int id);

Task<StationerySearchAdvancedViewModel> SearchAsync(string? keyword, string? stockStatus);
}