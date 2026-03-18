using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.GetFollowing;

public class GetFollowingQuery : Command<GetFollowingQueryResponse>
{
    public Guid UserId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public GetFollowingQuery(Guid userId, int pageNumber, int pageSize)
    {
        UserId = userId;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
