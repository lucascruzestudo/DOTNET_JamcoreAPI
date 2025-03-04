using Project.Application.Common.Models;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetTracksByTag;

public record GetTracksByTagQueryResponse
{
    public PaginatedList<TrackViewModel> Tracks { get; set; } = default!;
}

