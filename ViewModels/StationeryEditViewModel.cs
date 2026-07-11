using Microsoft.AspNetCore.Http;

namespace MiniStationery.Mvc.ViewModels;

public class StationeryEditViewModel : StationeryCreateViewModel
{
    public int Id { get; set; }
    public string RowVersion { get; set; } = string.Empty;
    public string? ExistingImageUrl { get; set; }
    public IFormFile? ImageFile { get; set; }
}