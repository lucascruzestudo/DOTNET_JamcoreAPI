namespace Project.Domain.Interfaces.Services;

 public interface ISupabaseService
    {
        Task<string> UploadFileAsync(byte[] imageBytes, string fileName, string bucketName);
        Task<string> GetPublicUrlAsync(string fileName, string bucketName);
        Task<string> GetSignedUrlAsync(string fileName, string bucketName, int expiresIn);
        Task<bool> DeleteFileAsync(string fileName, string bucketName);
    }