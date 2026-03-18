namespace Project.Application.Features.Commands.UnfollowUser;

public record UnfollowUserCommandRequest
{
    /// <summary>
    /// Accepts either the target User.Id or the target UserProfile.Id.
    /// </summary>
    public Guid FollowedUserId { get; init; }
}
