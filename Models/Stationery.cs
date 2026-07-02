namespace MiniStationery.Api.Models;

public class Stationery
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";
    public string Brand { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}