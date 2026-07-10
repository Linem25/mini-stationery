using Microsoft.EntityFrameworkCore;
using MiniStationery.Mvc.Data;
using MiniStationery.Mvc.Models;
using MiniStationery.Mvc.ViewModels;

namespace MiniStationery.Mvc.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _context;
    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task CreateOrderAsync(OrderCreateViewModel model)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var stationery = await _context.Stationeries.FirstOrDefaultAsync(s => s.Id == model.StationeryId);
            if (stationery == null) throw new Exception("Stationery not found");
            if (stationery.Stock < model.Quantity) throw new Exception("Not enough stock");
            var order = new Order
            {
                CreatedAt = DateTime.Now,
                TotalAmount = stationery.Price * model.Quantity
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            var item = new OrderItem
            {
                OrderId = order.Id,
                StationeryId = stationery.Id,
                Quantity = model.Quantity,
                UnitPrice = stationery.Price
            };
            _context.OrderItems.Add(item);
            stationery.Stock -= model.Quantity;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}