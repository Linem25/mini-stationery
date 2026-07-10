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

    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}