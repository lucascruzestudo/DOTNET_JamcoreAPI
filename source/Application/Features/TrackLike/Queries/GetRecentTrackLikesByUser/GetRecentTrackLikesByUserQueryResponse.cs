using Project.Application.Common.Models;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Queries.GetRecentTrackLikesByUser;

public class GetRecentTrackLikesByUserQueryResponse
{
    public PaginatedList<TrackViewModel> Tracks { get; set; } = default!;
}