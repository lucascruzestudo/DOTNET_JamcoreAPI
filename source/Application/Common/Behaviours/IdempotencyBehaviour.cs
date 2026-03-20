using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Project.Application.Common.Exceptions;
using Project.Application.Common.Interfaces;
using Project.Domain.Interfaces.Services;

namespace Project.Application.Common.Behaviours;

/// <summary>
/// MediatR pipeline behavior that prevents duplicate command processing.
/// Applies only to commands implementing <see cref="IIdempotentCommand"/>.
///
/// A fingerprint key is derived from:
///   idempotency:{CommandName}:{UserId}:{SHA256(serialized payload)}
///
/// If the same key already exists in Redis (within the TTL window), the request
/// is rejected with an <see cref="IdempotencyException"/> → HTTP 409 Conflict.
/// On handler failure the key is removed so the client can retry freely.
/// </summary>
public class IdempotencyBehaviour<TRequest, TResponse>(
    IRedisService redis,
    IUser user,
    ILogger<IdempotencyBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IIdempotentCommand
{
    private static readonly TimeSpan DefaultTtl = TimeSpan.FromSeconds(30);

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var json     = JsonSerializer.Serialize(request);
        var hash     = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json)));
        var userId   = user.Id?.ToString() ?? "anon";
        var cacheKey = $"idempotency:{typeof(TRequest).Name}:{userId}:{hash}";

        var existing = await redis.GetAsync<string>(cacheKey);
        if (existing != null)
        {
            logger.LogWarning(
                "Idempotency: duplicate {Request} blocked for user {UserId}",
                typeof(TRequest).Name, userId);

            throw new IdempotencyException();
        }

        // Reserve the key before processing to block concurrent duplicates.
        await redis.SetAsync(cacheKey, "processing", DefaultTtl);

        try
        {
            return await next();
        }
        catch
        {
            // Remove the key on failure so the client can retry the same payload.
            await redis.DeleteAsync(cacheKey);
            throw;
        }
    }
}
