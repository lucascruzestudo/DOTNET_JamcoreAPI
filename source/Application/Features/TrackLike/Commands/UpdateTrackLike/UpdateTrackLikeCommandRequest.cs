namespace Project.Application.Features.Commands.UpdateTrackLike;

public record UpdateTrackLikeCommandRequest
{
    public Guid TrackId { get; set; }
}