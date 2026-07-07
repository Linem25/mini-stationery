using System.ComponentModel.DataAnnotations;

namespace MiniStationery.Mvc.Models;

public class Stationery
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string SupplyCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}