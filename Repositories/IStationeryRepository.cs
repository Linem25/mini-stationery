using MiniStationery.Mvc.Models;

namespace MiniStationery.Mvc.Repositories;

public interface IStationeryRepository
{
    Task<List<Stationery>> GetAllAsync();
    Task<List<Stationery>> GetAllReadOnlyAsync();
    Task<Stationery?> GetByIdAsync(int id);
    Task AddAsync(Stationery stationery);
    Task SaveChangesAsync();
}