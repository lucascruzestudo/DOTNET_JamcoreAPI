namespace Project.Domain.Interfaces.Services;

 public interface ISupabaseService
    {
        Task<string> UploadFileAsync(byte[] imageBytes, string fileName, string bucketName);
        Task<string> GetPublicUrlAsync(string fileName);
        Task<bool> DeleteFileAsync(string fileName);
    }