using Project.Application.Common.Models;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.SearchTracks;

public record SearchTracksQueryResponse
{
    public PaginatedList<TrackViewModel> Tracks { get; set; } = default!;
}
