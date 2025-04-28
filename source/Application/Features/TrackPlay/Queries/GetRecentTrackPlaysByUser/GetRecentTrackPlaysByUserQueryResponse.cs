using Project.Application.Common.Models;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Queries.GetRecentTrackPlaysByUser;

public class GetRecentTrackPlaysByUserQueryResponse
{
    public PaginatedList<TrackViewModel> Tracks { get; set; } = default!;
}