namespace MiniStationery.Mvc.ViewModels;

public class StationerySearchAdvancedViewModel
{
    public string? Keyword { get; set; }
    public string? StockStatus { get; set; }  // "low", "out", "available", hoặc null = tất cả
    public List<StationeryListItemViewModel> Items { get; set; } = new();
}