using Microsoft.EntityFrameworkCore;
using MiniStationery.Mvc.Data;
using MiniStationery.Mvc.Models;

namespace MiniStationery.Mvc.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Order?> GetByIdAsync(int id)
        => _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.Id == id);

    public async Task AddAsync(Order order)
        => await _context.Orders.AddAsync(order);

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
}