using StackExchange.Redis;
using System.Text.Json;
using Project.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Project.Infrastructure.Cache;

public class RedisService(IConnectionMultiplexer redis, ILogger<RedisService> logger) : IRedisService
{
    private readonly IDatabase _database = redis.GetDatabase();
    private readonly ILogger<RedisService> _logger = logger;

    public async Task SetAsync<T>(string key, T value, TimeSpan expirationTime)
    {
        try
        {
            if (!redis.IsConnected)
            {
                _logger.LogWarning("Redis is not connected. Skipping cache operation for key: {Key}", key);
                return;
            }

            var jsonValue = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, jsonValue, expirationTime);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Redis operation timeout for key: {Key}. Cache operation skipped.", key);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "Redis connection error for key: {Key}. Cache operation skipped.", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unexpected error in Redis SetAsync for key: {Key}. Cache operation skipped.", key);
        }
    }

    public async Task<string?> GetAsync<T>(string key)
    {
        try
        {
            if (!redis.IsConnected)
            {
                _logger.LogWarning("Redis is not connected. Skipping cache retrieval for key: {Key}", key);
                return null;
            }

            return await _database.StringGetAsync(key);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Redis operation timeout for key: {Key}. Returning null.", key);
            return null;
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "Redis connection error for key: {Key}. Returning null.", key);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unexpected error in Redis GetAsync for key: {Key}. Returning null.", key);
            return null;
        }
    }

    public async Task DeleteAsync(string key)
    {
        try
        {
            if (!redis.IsConnected)
            {
                _logger.LogWarning("Redis is not connected. Skipping cache deletion for key: {Key}", key);
                return;
            }

            await _database.KeyDeleteAsync(key);
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Redis operation timeout for key: {Key}. Cache deletion skipped.", key);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "Redis connection error for key: {Key}. Cache deletion skipped.", key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unexpected error in Redis DeleteAsync for key: {Key}. Cache deletion skipped.", key);
        }
    }

    public async Task DeleteByPrefixAsync(string prefix)
    {
        try
        {
            if (!redis.IsConnected)
            {
                _logger.LogWarning("Redis is not connected. Skipping prefix deletion for prefix: {Prefix}", prefix);
                return;
            }

            var endpoints = redis.GetEndPoints();
            if (endpoints.Length == 0) return;

            var server = redis.GetServer(endpoints[0]);
            var keys = server.Keys(pattern: $"{prefix}*").ToArray();

            if (keys.Length > 0)
            {
                await _database.KeyDeleteAsync(keys);
                _logger.LogDebug("Redis: deleted {Count} keys with prefix '{Prefix}'.", keys.Length, prefix);
            }
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogWarning(ex, "Redis operation timeout for prefix: {Prefix}. Cache deletion skipped.", prefix);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogWarning(ex, "Redis connection error for prefix: {Prefix}. Cache deletion skipped.", prefix);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unexpected error in Redis DeleteByPrefixAsync for prefix: {Prefix}. Cache deletion skipped.", prefix);
        }
    }
}

