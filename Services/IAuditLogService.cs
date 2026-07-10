namespace MiniStationery.Mvc.Services;

public interface IAuditLogService
{
    Task LogAsync(string action, string entityName, string? entityId, string result, string? note = null);
}