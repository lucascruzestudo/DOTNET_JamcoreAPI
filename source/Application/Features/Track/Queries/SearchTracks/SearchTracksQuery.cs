using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.SearchTracks;

public class SearchTracksQuery : Command<SearchTracksQueryResponse>
{
    public string Search { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;

    public SearchTracksQuery(string search, int pageNumber, int pageSize)
    {
        Search = search;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
