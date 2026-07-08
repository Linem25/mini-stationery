namespace MiniStationery.Mvc.ViewModels;

public class HealthCheckViewModel
{
    public string PageTitle { get; set; } = "";
    public string OverallStatus { get; set; } = "";
    public List<HealthCheckItemViewModel> Checks { get; set; } = new();
}

public class HealthCheckItemViewModel
{
    public string Name { get; set; } = "";
    public string Status { get; set; } = "";
    public string Description { get; set; } = "";
}