namespace MiniStationery.Mvc.ViewModels;

public class StationeryAdjustStockViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int CurrentStock { get; set; }
    public int Adjustment { get; set; }  
    public string RowVersion { get; set; } = "";
}