using Project.Application.Common.Models;
using Project.Domain.ViewModels;

namespace Project.Application.Features.Commands.GetTrack;

public record GetTrackQueryResponse
{
    public TrackViewModel Track { get; set; } = default!;
}

