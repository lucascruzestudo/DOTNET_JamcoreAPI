using Project.Application.Common.Models;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Queries.GetFeed;

public record GetFeedQueryResponse
{
    public PaginatedList<TrackViewModel> Tracks { get; set; } = default!;
    public List<TrackViewModel> RecentPlays { get; set; } = [];
    public List<TrackViewModel> RecentLikes { get; set; } = [];
}
