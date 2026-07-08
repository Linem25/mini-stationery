namespace MiniStationery.Mvc.ViewModels;

public class StationeryTrashItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? DeletedAt { get; set; }
}