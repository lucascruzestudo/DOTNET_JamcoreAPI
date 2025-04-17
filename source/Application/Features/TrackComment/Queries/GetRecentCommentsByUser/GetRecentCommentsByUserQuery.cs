using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.GetRecentCommentsByUser;

public class GetRecentCommentsByUserQuery : Command<GetRecentCommentsByUserQueryResponse>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 3;
    public Guid UserId { get; init; }
    public GetRecentCommentsByUserQuery(int pageNumber, int pageSize, Guid userId)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        UserId = userId;
    }
}
