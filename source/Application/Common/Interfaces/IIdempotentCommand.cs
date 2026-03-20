namespace Project.Application.Common.Interfaces;

/// <summary>
/// Marker interface for commands that require idempotency protection.
/// Commands implementing this interface are checked against a short-lived Redis key
/// derived from the command name, user ID, and a SHA-256 hash of the payload.
/// A duplicate call within the TTL window returns HTTP 409 Conflict.
/// </summary>
public interface IIdempotentCommand { }
