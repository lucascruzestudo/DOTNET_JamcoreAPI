using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.GetTracksByTag;

public class GetTracksByTagQuery : Command<GetTracksByTagQueryResponse>
{
    public string Tag { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public GetTracksByTagQuery(string tag, int pageNumber, int pageSize)
    {
        Tag = tag;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
