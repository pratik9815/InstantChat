using InstantChat.Application.Common.Interfaces;

namespace InstantChat.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    public Task<bool> DeleteFileAsync(string filePath)
    {
        throw new NotImplementedException();
    }

    public Task<bool> FileExistsAsync(string filePath)
    {
        throw new NotImplementedException();
    }

    public Task<byte[]> GetFileAsync(string filePath)
    {
        throw new NotImplementedException();
    }

    public string GetFileUrl(string filePath)
    {
        throw new NotImplementedException();
    }

    public bool IsValidImageFile(string fileName, long fileSize)
    {
        throw new NotImplementedException();
    }

    public Task<string> SaveFileAsync(Stream fileStream, string fileName, string folder = "images")
    {
        throw new NotImplementedException();
    }
}
