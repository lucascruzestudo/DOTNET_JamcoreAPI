using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.GetTracksByUser;

public class GetTracksByUserQuery : Command<GetTracksByUserQueryResponse>
{
    public string Username { get; init; } = string.Empty;
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public GetTracksByUserQuery(string username, int pageNumber, int pageSize)
    {
        Username = username;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
