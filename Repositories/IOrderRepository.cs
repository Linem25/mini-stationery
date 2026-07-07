using MiniStationery.Mvc.Models;

namespace MiniStationery.Mvc.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task AddAsync(Order order);
    Task SaveChangesAsync();
}