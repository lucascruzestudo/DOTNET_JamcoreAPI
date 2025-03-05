using Project.Application.Common.Models;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetTracksByUser;

public record GetTracksByUserQueryResponse
{
    public PaginatedList<TrackViewModel> Tracks { get; set; } = default!;
}

