namespace MiniStationery.Mvc.ViewModels;

public class StationerySearchViewModel
{
    public string Keyword { get; set; } = "";

    public decimal? MinPrice { get; set; }

    public List<StationeryListItemViewModel> Items { get; set; } = new();
}