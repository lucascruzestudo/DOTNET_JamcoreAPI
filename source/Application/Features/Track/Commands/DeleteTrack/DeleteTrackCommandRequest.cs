namespace Project.Application.Features.Commands.DeleteTrack;

public record DeleteTrackCommandRequest
{
    public Guid? TrackId { get; set; }
}
