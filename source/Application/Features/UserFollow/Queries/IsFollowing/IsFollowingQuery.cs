using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.IsFollowing;

public class IsFollowingQuery : Command<IsFollowingQueryResponse>
{
    public Guid FollowedUserId { get; init; }

    public IsFollowingQuery(Guid followedUserId)
    {
        FollowedUserId = followedUserId;
    }
}
