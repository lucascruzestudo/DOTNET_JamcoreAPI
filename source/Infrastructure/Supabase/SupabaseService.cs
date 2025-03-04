using Supabase;
using Microsoft.Extensions.Configuration;
using Project.Domain.Interfaces.Services;

namespace Project.Infrastructure.Supabase
{
    public class SupabaseService : ISupabaseService
    {
        private readonly Client _supabase;
        private readonly string _bucketName;

        public SupabaseService(IConfiguration configuration)
        {
            var url = configuration["Supabase:Url"];
            var key = configuration["Supabase:Key"];
            _bucketName = configuration["Supabase:Bucket"]!;

            var options = new SupabaseOptions { AutoConnectRealtime = true };
            _supabase = new Client(url!, key, options);
        }

        public async Task<string> UploadFileAsync(byte[] imageBytes, string fileName, string bucketName)
        {

            await _supabase.InitializeAsync();
            var storage = _supabase.Storage;
            var bucket = storage.From(bucketName);

            using var stream = new MemoryStream(imageBytes);
            var response = await bucket.Upload(imageBytes, fileName);
            var publicUrl = bucket.GetPublicUrl(fileName);
            return publicUrl;
        }

        public async Task<string> GetPublicUrlAsync(string fileName, string bucketName)
        {
            await _supabase.InitializeAsync();
            var storage = _supabase.Storage;
            var bucket = storage.From(bucketName);

            return bucket.GetPublicUrl(fileName);
        }

        public async Task<bool> DeleteFileAsync(string fileName, string bucketName)
        {
            await _supabase.InitializeAsync();
            var storage = _supabase.Storage;
            var bucket = storage.From(bucketName);
            await bucket.Remove([fileName]);
            return true;
        }
    }
}

