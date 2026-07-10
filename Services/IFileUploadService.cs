namespace MiniStationery.Mvc.Services;

public interface IFileUploadService
{
    Task<string> SaveStationeryImageAsync(IFormFile file);
}