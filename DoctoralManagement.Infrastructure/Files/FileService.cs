using DoctoralManagement.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using System.Diagnostics;

namespace DoctoralManagement.Infrastructure.Files
{
    public class FileService : IFileService
    {
        private static readonly FileExtensionContentTypeProvider ContentTypeProvider =
            new FileExtensionContentTypeProvider();

        public byte[] ReadFile(string filePath)
        {
            // If filePath is just a filename, reconstruct the full path
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", filePath);
            }

            Debug.WriteLine($"[FileService] Attempting to read file: {filePath}");
            Debug.WriteLine($"[FileService] File exists: {File.Exists(filePath)}");
            Debug.WriteLine($"[FileService] Current directory: {Directory.GetCurrentDirectory()}");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found.", filePath);
            }
            return File.ReadAllBytes(filePath);
        }

        public bool FileExists(string filePath)
        {
            // If filePath is just a filename, reconstruct the full path
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", filePath);
            }

            Debug.WriteLine($"[FileService] Checking if file exists: {filePath}");
            return File.Exists(filePath);
        }

        public string GetContentType(string filePath)
        {
            if (!ContentTypeProvider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

        public bool DeleteFile(string filePath)
        {
            if (!Path.IsPathRooted(filePath))
            {
                filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", filePath);
            }

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }

        public string GetFilePath(IFormFile file)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "Uploads", file.FileName);
        }

        public long GetFileSize(IFormFile file)
        {
            return file.Length;
        }

        public string UploadFile(IFormFile file, string FileName)
        {
            List<string> validExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".pdf", ".docx" };
            string fileExtension = Path.GetExtension(file.FileName);
            if (!validExtensions.Contains(fileExtension))
            {
                return "Invalid file type. Only .jpg, .jpeg, .png, .pdf, and .docx files are allowed.";
            }

            long size = file.Length;
            if (size > (5 * 1024 * 1024))
            {
                return "File size exceeds the 5MB limit.";
            }
            string name = $"{FileName}{fileExtension}";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            // Ensure Uploads directory exists
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            using FileStream fileStream = new FileStream(Path.Combine(filePath, name), FileMode.Create);
            file.CopyTo(fileStream);

            return name;
        }
    }
}

//using DoctoralManagement.Domain.Interfaces;
//using Microsoft.AspNetCore.Http;

//namespace DoctoralManagement.Infrastructure.Files
//{
//    public class FileService : IFileService
//    {
//        public bool DeleteFile(string filePath)
//        {
//            if (File.Exists(filePath))
//            {
//                File.Delete(filePath);
//                return true;
//            }
//            return false;
//        }

//        public bool FileExists(string filePath)
//        {
//            throw new NotImplementedException();
//        }

//        public string GetContentType(string filePath)
//        {
//            throw new NotImplementedException();
//        }

//        public string GetFilePath(IFormFile file)
//        {
//            return Path.Combine(Directory.GetCurrentDirectory(), "Uploads", file.FileName);
//        }

//        public long GetFileSize(IFormFile file)
//        {
//            return file.Length;
//        }

//        public byte[] ReadFile(string filePath)
//        {
//            if (!File.Exists(filePath))
//            {
//                throw new FileNotFoundException("File not found.", filePath);
//            }
//            return File.ReadAllBytes(filePath);
//        }

//        public string UploadFile(IFormFile file, string FileName)
//        {
//            List<string> validExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".pdf", ".docx" };
//            string fileExtension = Path.GetExtension(file.FileName);
//            if (!validExtensions.Contains(fileExtension))
//            {
//                return "Invalid file type. Only .jpg, .jpeg, .png, .pdf, and .docx files are allowed.";
//            }

//            long size = file.Length;
//            if (size > (5 * 1024 * 1024))
//            {
//                return "File size exceeds the 5MB limit.";
//            }
//            string name = $"{FileName}{fileExtension}";
//            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
//            using FileStream fileStream = new FileStream(Path.Combine(filePath, name), FileMode.Create);
//            file.CopyTo(fileStream);

//            return name;
//        }
//    }
//}