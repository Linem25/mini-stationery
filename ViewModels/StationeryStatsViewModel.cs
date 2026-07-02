namespace MiniStationery.Mvc.ViewModels;

public class StationeryStatsViewModel
{
    public int TotalItems { get; set; }

    public int TotalQuantity { get; set; }

    public decimal TotalInventoryValue { get; set; }

    public int OutOfStockCount { get; set; }

    public int NeedReorderCount { get; set; }

    public string TotalInventoryValueText => $"{TotalInventoryValue:N0} VND";
}