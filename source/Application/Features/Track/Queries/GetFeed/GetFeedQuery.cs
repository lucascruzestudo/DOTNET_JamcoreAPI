using Project.Domain.Notifications;

namespace Project.Application.Features.Queries.GetFeed;

public class GetFeedQuery : Command<GetFeedQueryResponse>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public GetFeedQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
