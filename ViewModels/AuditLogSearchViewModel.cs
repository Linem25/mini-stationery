using MiniStationery.Mvc.Models;

namespace MiniStationery.Mvc.ViewModels;

public class AuditLogSearchViewModel
{
    public string? UserName { get; set; }
    public string? ActionName { get; set; }
    public string? Result { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<AuditLog> Items { get; set; } = new();
}