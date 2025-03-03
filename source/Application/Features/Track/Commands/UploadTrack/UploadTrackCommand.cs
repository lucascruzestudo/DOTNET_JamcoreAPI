using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.UploadTrack;

public class UploadTrackCommand(UploadTrackCommandRequest request, byte[]? track = null, byte[]? image = null) : Command<UploadTrackCommandResponse>
{
    public UploadTrackCommandRequest Request { get; set; } = request;
    public byte[]? Track { get; set; } = track;
    public byte[]? Image { get; set; } = image;
}
