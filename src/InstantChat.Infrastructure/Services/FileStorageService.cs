using InstantChat.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace InstantChat.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly IWebHostEnvironment _env;

    public FileStorageService(IWebHostEnvironment env)
    {
        _env = env;
    }
    public async Task<bool> DeleteFileAsync(string filePath)
    {
        string fullPath = Path.Combine(_env.WebRootPath, filePath.Replace("/", "\\"));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
            return await Task.FromResult(true);
        }

        return await Task.FromResult(false);
    }

    public async Task<bool> FileExistsAsync(string filePath)
    {
        string fullPath = Path.Combine(_env.WebRootPath, filePath.Replace("/", "\\"));
        return await Task.FromResult(File.Exists(fullPath));
    }

    public async Task<byte[]> GetFileAsync(string filePath)
    {
        string fullPath = Path.Combine(_env.WebRootPath, filePath.Replace("/", "\\"));
        if (!File.Exists(fullPath))
            return null;

        return await File.ReadAllBytesAsync(fullPath);
    }

    public string GetFileUrl(string filePath)
    {
        // Return relative URL
        return "/" + filePath.Replace("\\", "/");
    }

    public bool IsValidImageFile(string fileName, long fileSize)
    {
        // Example validation: check extension and size (max 5 MB)
        string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        string extension = Path.GetExtension(fileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(extension) || Array.IndexOf(permittedExtensions, extension) < 0)
            return false;

        const long maxFileSize = 5 * 1024 * 1024; // 5 MB
        if (fileSize > maxFileSize)
            return false;

        return true;
    }

    public async Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder = "images")
    {
        if (fileStream == null || fileStream.Length == 0)
            throw new ArgumentException("File stream cannot be null or empty", nameof(fileStream));

        // Ensure folder exists
        string folderPath = Path.Combine(_env.WebRootPath, folder);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        // Generate unique file name
        string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        string filePath = Path.Combine(folderPath, uniqueFileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await fileStream.CopyToAsync(stream);
        }
        // Return relative path for access via URL
        return Path.Combine(folder, uniqueFileName).Replace("\\", "/");
    }
}
