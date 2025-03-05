using Project.Application.Common.Models;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetRecentTracks;

public record GetRecentTracksQueryResponse
{
    public PaginatedList<TrackViewModel> Tracks { get; set; } = default!;
}

