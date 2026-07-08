namespace MiniStationery.Mvc.ViewModels;

public class DashboardViewModel
{
    public int TotalStationeries { get; set; }
    public int ActiveStationeries { get; set; }
    public int DeletedStationeries { get; set; }
    public int LogsToday { get; set; }
}