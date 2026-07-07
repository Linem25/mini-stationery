namespace MiniStationery.Mvc.ViewModels;

public class DataHealthViewModel
{
    public bool SeedDataEnabled { get; set; }
    public int TotalCategories { get; set; }
    public int TotalStationeries { get; set; }
    public int TotalOrders { get; set; }
    public string DatabaseProvider { get; set; } = "";
}