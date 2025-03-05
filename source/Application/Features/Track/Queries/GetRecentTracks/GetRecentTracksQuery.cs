using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.GetRecentTracks;

public class GetRecentTracksQuery : Command<GetRecentTracksQueryResponse>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public GetRecentTracksQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
