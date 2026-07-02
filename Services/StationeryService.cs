using MiniStationery.Api.Models;

namespace MiniStationery.Api.Services;

public class StationeryService
{
    private readonly List<Stationery> _stationeries =
    [
        new Stationery { Id = 1, Name = "Bút bi Thiên Long TL-027", Category = "Bút viết", Brand = "Thiên Long", Price = 3500, Quantity = 120 },
        new Stationery { Id = 2, Name = "Vở kẻ ngang Campus 200 trang", Category = "Sổ vở", Brand = "Campus", Price = 18000, Quantity = 4 },
        new Stationery { Id = 3, Name = "Bút highlight Stabilo Boss", Category = "Bút viết", Brand = "Stabilo", Price = 22000, Quantity = 0 },
        new Stationery { Id = 4, Name = "Kéo văn phòng Deli", Category = "Dụng cụ", Brand = "Deli", Price = 15000, Quantity = 30 },
        new Stationery { Id = 5, Name = "Bộ ghim bấm số 10", Category = "Dụng cụ", Brand = "Kokuyo", Price = 9000, Quantity = 2 }
    ];

    public List<Stationery> GetAll() => _stationeries;

    public object GetStats()
    {
        var totalItems = _stationeries.Count;
        var totalQuantity = _stationeries.Sum(x => x.Quantity);
        var totalValue = _stationeries.Sum(x => x.Price * x.Quantity);
        var availableItems = _stationeries.Count(x => x.Quantity > 0);

        return new
        {
            TotalItems = totalItems,
            TotalQuantity = totalQuantity,
            TotalValue = totalValue,
            AvailableItems = availableItems
        };
    }

    public string GetStockStatus(int quantity)
    {
        if (quantity <= 0) return "Out of stock";
        if (quantity <= 5) return "Low stock";
        return "Available";
    }
}