using Microsoft.AspNetCore.Http;

namespace DoctoralManagement.Domain.Interfaces
{
    public interface IFileService
    {
        string UploadFile(IFormFile file, string FileName);
        string GetFilePath(IFormFile file);
        long GetFileSize(IFormFile file);
        bool DeleteFile(string filePath);     
        byte[] ReadFile(string filePath);
        bool FileExists(string filePath);
        string GetContentType(string filePath);
    }
}
