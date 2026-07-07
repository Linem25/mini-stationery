using Microsoft.EntityFrameworkCore;
using MiniStationery.Mvc.Data;
using MiniStationery.Mvc.Models;

namespace MiniStationery.Mvc.Repositories;

public class StationeryRepository : IStationeryRepository
{
    private readonly AppDbContext _context;

    public StationeryRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Stationery>> GetAllAsync()
        => _context.Stationeries.Include(s => s.Category).ToListAsync();

    public Task<List<Stationery>> GetAllReadOnlyAsync()
        => _context.Stationeries.Include(s => s.Category).AsNoTracking().ToListAsync();

    public Task<Stationery?> GetByIdAsync(int id)
        => _context.Stationeries.Include(s => s.Category).FirstOrDefaultAsync(s => s.Id == id);

    public async Task AddAsync(Stationery stationery)
        => await _context.Stationeries.AddAsync(stationery);

    public Task SaveChangesAsync()
        => _context.SaveChangesAsync();
}