namespace MiniStationery.Mvc.Services;

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _environment;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxFileSize = 2 * 1024 * 1024;

    public FileUploadService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveStationeryImageAsync(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!AllowedExtensions.Contains(ext))
        {
            throw new InvalidOperationException("Định dạng file không được phép. Chỉ chấp nhận .jpg, .jpeg, .png, .webp.");
        }

        if (file.Length > MaxFileSize)
        {
            throw new InvalidOperationException("File vượt quá 2MB.");
        }

        if (file.Length == 0)
        {
            throw new InvalidOperationException("File rỗng.");
        }

        var safeName = $"{Guid.NewGuid():N}{ext}";
        var folder = Path.Combine(_environment.WebRootPath, "uploads", "stationery");
        Directory.CreateDirectory(folder);
        var fullPath = Path.Combine(folder, safeName);

        using var stream = new FileStream(fullPath, FileMode.CreateNew); 
        await file.CopyToAsync(stream);

        return $"/uploads/stationery/{safeName}";
    }

    public void DeleteStationeryImage(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl)) return;

        var uploadsRoot = Path.Combine(_environment.WebRootPath, "uploads", "stationery");
        var fileName = Path.GetFileName(imageUrl); 
        var fullPath = Path.Combine(uploadsRoot, fileName);

       
        var resolvedPath = Path.GetFullPath(fullPath);
        var resolvedRoot = Path.GetFullPath(uploadsRoot);

        if (!resolvedPath.StartsWith(resolvedRoot, StringComparison.OrdinalIgnoreCase))
        {
            return; 
        }

        if (File.Exists(resolvedPath))
        {
            File.Delete(resolvedPath);
        }
    }
}