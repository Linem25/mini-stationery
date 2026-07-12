namespace MiniStationery.Mvc.ViewModels;

public class DashboardViewModel
{
    public int TotalStationeries { get; set; }
    public int TotalOrders { get; set; }
    public int TotalAuditLogs { get; set; }
    public int SecurityControlsEnabled { get; set; }
    public int SecurityControlsTotal { get; set; } = 8;
    public int AccessDeniedToday { get; set; }
public int SensitiveActionsToday { get; set; }
public int UploadRejectedToday { get; set; }
}