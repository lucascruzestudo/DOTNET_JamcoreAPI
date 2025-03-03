namespace Project.Domain.Interfaces.Services;

public interface IRedisService
{
    Task SetAsync<T>(string key, T value, TimeSpan expirationTime);
    Task<string?> GetAsync<T>(string key);
    Task DeleteAsync(string key);
}
