namespace Project.Application.Features.Commands.FollowUser;

public record FollowUserCommandRequest
{
    /// <summary>
    /// Accepts either the target User.Id or the target UserProfile.Id.
    /// </summary>
    public Guid FollowedUserId { get; init; }
}
