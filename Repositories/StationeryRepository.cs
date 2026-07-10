using Microsoft.EntityFrameworkCore;
using MiniStationery.Mvc.Data;
using MiniStationery.Mvc.Models;

namespace MiniStationery.Mvc.Repositories;

public class StationeryRepository : IStationeryRepository
{
    private readonly ApplicationDbContext _context;

    public StationeryRepository(ApplicationDbContext context)
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

    public async Task<List<Stationery>> FilterAsync(int? categoryId, decimal? minPrice, decimal? maxPrice)
    {
        var query = _context.Stationeries.Include(s => s.Category).AsNoTracking().AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(s => s.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(s => s.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(s => s.Price <= maxPrice.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<List<Stationery>> SearchAsync(string? keyword, string? stockStatus)
{

    var query = _context.Stationeries
        .Include(s => s.Category)
        .AsNoTracking()
        .AsQueryable();

    if (!string.IsNullOrWhiteSpace(keyword))
    {
        
        query = query.Where(s =>
            s.Name.Contains(keyword) ||
            s.SupplyCode.Contains(keyword));
    }

    query = stockStatus switch
    {
        "out" => query.Where(s => s.Stock <= 0),
        "low" => query.Where(s => s.Stock > 0 && s.Stock <= 10),
        "available" => query.Where(s => s.Stock > 10),
        _ => query
    };

    return await query.ToListAsync();
}

}