namespace Project.Application.Features.Queries.GetRecentTrackPlaysByUser;

public class GetRecentTrackPlaysByUserQuery : IRequest<GetRecentTrackPlaysByUserQueryResponse?>
{
    public Guid UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetRecentTrackPlaysByUserQuery(Guid userId, int pageNumber, int pageSize)
    {
        UserId = userId;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}