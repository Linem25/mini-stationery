namespace MiniStationery.Mvc.Services;

public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _environment;

    public FileUploadService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveStationeryImageAsync(IFormFile file)
    {
        var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowed.Contains(ext))
        {
            throw new InvalidOperationException("File type is not allowed.");
        }

        if (file.Length > 2 * 1024 * 1024)
        {
            throw new InvalidOperationException("File is too large.");
        }

        var safeName = $"{Guid.NewGuid():N}{ext}";
        var folder = Path.Combine(_environment.WebRootPath, "uploads", "stationery");
        Directory.CreateDirectory(folder);
        var path = Path.Combine(folder, safeName);

        using var stream = new FileStream(path, FileMode.CreateNew);
        await file.CopyToAsync(stream);

        return $"/uploads/stationery/{safeName}";
    }
}