using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.UpdateUserVolume;

public class UpdateUserVolumeCommand : Command<UpdateUserVolumeCommandResponse>
{
    public float Volume { get; init; }

    public UpdateUserVolumeCommand(float volume)
    {
        Volume = volume;
    }
}
