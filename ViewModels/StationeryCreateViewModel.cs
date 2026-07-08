using System.ComponentModel.DataAnnotations;

namespace MiniStationery.Mvc.ViewModels;

public class StationeryCreateViewModel
{
    [Required(ErrorMessage = "Tên mặt hàng là bắt buộc.")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mã hàng là bắt buộc.")]
    [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "Mã hàng chỉ gồm chữ in hoa, số và dấu -.")]
    public string SupplyCode { get; set; } = string.Empty;

    [Range(500, 100000000, ErrorMessage = "Giá phải từ 500 đến 100.000.000.")]
    public decimal Price { get; set; }

    [Range(0, 100000, ErrorMessage = "Tồn kho phải từ 0 đến 100.000.")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn nhóm mặt hàng.")]
    public int CategoryId { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}