using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.GetTracksByUser;

public class GetTracksByUserQuery : Command<GetTracksByUserQueryResponse>
{
    public Guid Id { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public GetTracksByUserQuery(Guid id, int pageNumber, int pageSize)
    {
        Id = id;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
