using System.ComponentModel.DataAnnotations;

namespace MiniStationery.Mvc.ViewModels;

public class StationeryCreateViewModel
{
    [Required(ErrorMessage = "Tên mặt hàng không được để trống")]
    [StringLength(100, ErrorMessage = "Tên mặt hàng không được vượt quá 100 ký tự")]
    public string Name { get; set; } = "";

    [Required(ErrorMessage = "Nhóm mặt hàng không được để trống")]
    public string Category { get; set; } = "";

    [Required(ErrorMessage = "Nhà cung cấp không được để trống")]
    public string Supplier { get; set; } = "";

    [Range(500, 100000000, ErrorMessage = "Giá bán phải từ 500 đến 100.000.000")]
    public decimal UnitPrice { get; set; }

    [Range(0, 10000, ErrorMessage = "Số lượng phải từ 0 đến 10.000")]
    public int Quantity { get; set; }

    [Range(0, 10000, ErrorMessage = "Mức tồn tối thiểu phải từ 0 đến 10.000")]
    public int MinStock { get; set; }
}