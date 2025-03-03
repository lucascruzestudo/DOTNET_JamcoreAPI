using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.UpsertProfile;

public class UpsertProfileCommand : Command<UpsertProfileCommandResponse>
{
    public UpsertProfileCommandRequest Request { get; set; }
    public UpsertProfileCommand(UpsertProfileCommandRequest request)
    {
        Request = request;
    }
}
