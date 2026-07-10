using MiniStationery.Mvc.Data;
using MiniStationery.Mvc.Models;

namespace MiniStationery.Mvc.Services;

public class AuditLogService : IAuditLogService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(string action, string entityName, string? entityId, string result, string? note = null)
    {
        var log = new AuditLog
        {
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            UserName = _httpContextAccessor.HttpContext?.User?.Identity?.Name,
            IpAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
            Result = result,
            Note = note,
            CreatedAt = DateTime.UtcNow
        };
        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}