namespace Project.Domain.Interfaces.Services;

 public interface ISupabaseService
    {
        Task<string> UploadFileAsync(byte[] imageBytes, string fileName);
        Task<string> GetPublicUrlAsync(string fileName);
        Task<bool> DeleteFileAsync(string fileName);
    }