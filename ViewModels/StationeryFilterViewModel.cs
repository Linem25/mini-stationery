namespace MiniStationery.Mvc.ViewModels;

public class StationeryFilterViewModel
{
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public List<StationeryListItemViewModel> Items { get; set; } = new();
    public List<CategoryOptionViewModel> Categories { get; set; } = new();
}

public class CategoryOptionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}