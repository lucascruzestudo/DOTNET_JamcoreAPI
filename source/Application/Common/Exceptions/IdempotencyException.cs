namespace Project.Application.Common.Exceptions;

public class IdempotencyException()
    : Exception("Duplicate request detected. Please wait a moment before retrying.")
{
}
