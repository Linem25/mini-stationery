
namespace MiniStationery.Mvc.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Stationery> Stationeries { get; set; } = new List<Stationery>();
}