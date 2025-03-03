using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.UpsertProfile;

public class UpsertProfileCommand : Command<UpsertProfileCommandResponse>
{
    public UpsertProfileCommandRequest Request { get; set; }
    public byte[]? Image { get; set; }
    public UpsertProfileCommand(UpsertProfileCommandRequest request, byte[]? image = null)
    {
        Request = request;
        Image = image;
    }
}
