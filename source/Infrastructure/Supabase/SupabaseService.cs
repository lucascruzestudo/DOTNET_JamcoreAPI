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

        public async Task<string> UploadFileAsync(byte[] fileBytes, string fileName, string bucketName)
        {
            await _supabase.InitializeAsync();
            var bucket = _supabase.Storage.From(bucketName);

            var response = await bucket.Upload(fileBytes, fileName, new global::Supabase.Storage.FileOptions
            {
                Upsert = true
            });

            if (string.IsNullOrEmpty(response))
                throw new Exception($"Supabase upload returned no path for file '{fileName}' in bucket '{bucketName}'.");

            return bucket.GetPublicUrl(fileName);
        }

        public async Task<string> GetPublicUrlAsync(string fileName, string bucketName)
        {
            await _supabase.InitializeAsync();
            var storage = _supabase.Storage;
            var bucket = storage.From(bucketName);

            return bucket.GetPublicUrl(fileName);
        }

        public async Task<string> GetSignedUrlAsync(string fileName, string bucketName, int expiresIn)
        {
            await _supabase.InitializeAsync();
            var bucket = _supabase.Storage.From(bucketName);
            return await bucket.CreateSignedUrl(fileName, expiresIn);
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

