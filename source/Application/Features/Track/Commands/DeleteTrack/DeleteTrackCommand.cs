using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.DeleteTrack;

public class DeleteTrackCommand(DeleteTrackCommandRequest request, byte[]? track = null, byte[]? image = null) : Command<DeleteTrackCommandResponse>
{
    public DeleteTrackCommandRequest Request { get; set; } = request;
    public byte[]? Track { get; set; } = track;
    public byte[]? Image { get; set; } = image;
}
