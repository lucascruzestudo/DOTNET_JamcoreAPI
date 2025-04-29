namespace Project.Application.Features.Queries.GetRecentTrackLikesByUser;

public class GetRecentTrackLikesByUserQuery : IRequest<GetRecentTrackLikesByUserQueryResponse?>
{
    public Guid UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    public GetRecentTrackLikesByUserQuery(Guid userId, int pageNumber, int pageSize)
    {
        UserId = userId;
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}