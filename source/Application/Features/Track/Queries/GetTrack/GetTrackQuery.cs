using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.GetTrack;

public class GetTrackQuery : Command<GetTrackQueryResponse>
{
    public Guid TrackId { get; set; }
    public GetTrackQuery(Guid trackId)
    {
        TrackId = trackId;
    }
}
