namespace MiniStationery.Mvc.ViewModels;

public class StationeryDetailViewModel
{
    public int Id { get; set; }

    public string Sku { get; set; } = "";

    public string Name { get; set; } = "";

    public string Category { get; set; } = "";

    public string Supplier { get; set; } = "";

    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }

    public int MinStock { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public string PriceText => $"{UnitPrice:N0} VND";

    public decimal InventoryValue => UnitPrice * Quantity;

    public string InventoryValueText => $"{InventoryValue:N0} VND";

    public string LastUpdatedText => LastUpdatedAt.ToString("dd/MM/yyyy HH:mm");

    public string StockStatus
    {
        get
        {
            if (Quantity <= 0)
            {
                return "Hết hàng";
            }

            if (Quantity <= MinStock)
            {
                return "Cần nhập thêm";
            }

            return "Còn hàng";
        }
    }

    public string ReorderSuggestion
    {
        get
        {
            if (Quantity <= 0)
            {
                return "Cần nhập hàng ngay vì sản phẩm đã hết.";
            }

            if (Quantity <= MinStock)
            {
                return $"Nên nhập thêm. Tồn kho hiện tại chỉ còn {Quantity}, mức tối thiểu là {MinStock}.";
            }

            return "Tồn kho đang ổn định, chưa cần nhập thêm.";
        }
    }
}