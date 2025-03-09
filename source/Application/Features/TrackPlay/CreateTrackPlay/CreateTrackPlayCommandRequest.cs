namespace Project.Application.Features.Commands.CreateTrackPlay;

public record CreateTrackPlayCommandRequest
{
    public Guid TrackId { get; set; }
}