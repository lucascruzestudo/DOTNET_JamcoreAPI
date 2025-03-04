using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.UpdateTrack;

public class UpdateTrackCommand(UpdateTrackCommandRequest request, byte[]? track = null, byte[]? image = null) : Command<UpdateTrackCommandResponse>
{
    public UpdateTrackCommandRequest Request { get; set; } = request;
    public byte[]? Track { get; set; } = track;
    public byte[]? Image { get; set; } = image;
}
