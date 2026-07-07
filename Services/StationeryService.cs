using MiniStationery.Mvc.Models;
using MiniStationery.Mvc.ViewModels;

namespace MiniStationery.Mvc.Services;

public class StationeryService
{
    private readonly List<Stationery> _stationeries = new()
    {
        new Stationery
        {
            Id = 1,
            Sku = "PEN-BIC-001",
            Name = "Bút bi Thiên Long TL-027",
            Category = "Bút viết",
            Supplier = "Thiên Long",
            UnitPrice = 3500,
            Quantity = 120,
            MinStock = 20,
            LastUpdatedAt = new DateTime(2025, 5, 9, 9, 12, 0)
        },
        new Stationery
        {
            Id = 2,
            Sku = "NTB-CPS-002",
            Name = "Vở kẻ ngang Campus 200 trang",
            Category = "Sổ vở",
            Supplier = "Campus Vietnam",
            UnitPrice = 18000,
            Quantity = 4,
            MinStock = 10,
            LastUpdatedAt = new DateTime(2025, 5, 9, 9, 12, 0)
        },
        new Stationery
        {
            Id = 3,
            Sku = "PEN-HLT-003",
            Name = "Bút highlight Stabilo Boss",
            Category = "Bút viết",
            Supplier = "Stabilo",
            UnitPrice = 22000,
            Quantity = 0,
            MinStock = 5,
            LastUpdatedAt = new DateTime(2025, 5, 9, 9, 12, 0)
        },
        new Stationery
        {
            Id = 4,
            Sku = "TL-SCS-004",
            Name = "Kéo văn phòng Deli",
            Category = "Dụng cụ",
            Supplier = "Deli Vietnam",
            UnitPrice = 15000,
            Quantity = 30,
            MinStock = 8,
            LastUpdatedAt = new DateTime(2025, 5, 9, 9, 12, 0)
        },
        new Stationery
        {
            Id = 5,
            Sku = "TL-STP-005",
            Name = "Bộ ghim bấm số 10",
            Category = "Dụng cụ",
            Supplier = "Kokuyo",
            UnitPrice = 9000,
            Quantity = 2,
            MinStock = 10,
            LastUpdatedAt = new DateTime(2025, 5, 9, 9, 12, 0)
        },
        new Stationery
        {
            Id = 6,
            Sku = "FLD-PPR-006",
            Name = "Bìa hồ sơ nhựa A4",
            Category = "Văn phòng",
            Supplier = "Deli Vietnam",
            UnitPrice = 7000,
            Quantity = 45,
            MinStock = 15,
            LastUpdatedAt = new DateTime(2025, 5, 9, 9, 12, 0)
        }
    };

    public List<Stationery> GetAll()
    {
        return _stationeries;
    }

    public Stationery? GetById(int id)
    {
        return _stationeries.FirstOrDefault(item => item.Id == id);
    }

    public StationeryStatsViewModel GetStats()
    {
        var totalItems = _stationeries.Count;

        var totalQuantity = _stationeries.Sum(item => item.Quantity);

        var totalInventoryValue = _stationeries.Sum(item =>
            item.UnitPrice * item.Quantity);

        var outOfStockCount = _stationeries.Count(item =>
            item.Quantity <= 0);

        var needReorderCount = _stationeries.Count(item =>
            item.Quantity > 0 && item.Quantity <= item.MinStock);

        return new StationeryStatsViewModel
        {
            TotalItems = totalItems,
            TotalQuantity = totalQuantity,
            TotalInventoryValue = totalInventoryValue,
            OutOfStockCount = outOfStockCount,
            NeedReorderCount = needReorderCount
        };
    }
    public List<Stationery> Search(string? keyword, decimal? minPrice)
{
    var query = _stationeries.AsEnumerable();

    if (!string.IsNullOrWhiteSpace(keyword))
    {
        query = query.Where(item =>
            item.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            item.Category.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            item.Sku.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    if (minPrice.HasValue)
    {
        query = query.Where(item => item.UnitPrice >= minPrice.Value);
    }

    return query.ToList();
}

public Stationery Create(StationeryCreateViewModel model)
{
    var newId = _stationeries.Count == 0
        ? 1
        : _stationeries.Max(item => item.Id) + 1;

    var stationery = new Stationery
    {
        Id = newId,
        Sku = $"NEW-{newId:000}",
        Name = model.Name,
        Category = model.Category,
        Supplier = model.Supplier,
        UnitPrice = model.UnitPrice,
        Quantity = model.Quantity,
        MinStock = model.MinStock,
        LastUpdatedAt = DateTime.Now
    };

    _stationeries.Add(stationery);

    return stationery;
}
}