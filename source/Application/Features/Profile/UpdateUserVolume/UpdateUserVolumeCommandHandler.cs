using System.Threading;
using System.Threading.Tasks;
using Project.Application.Common.Interfaces;
using Project.Application.Common.Localizers;
using Project.Domain.Entities;
using Project.Domain.Interfaces.Data.Repositories;
using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.UpdateUserVolume;

public class UpdateUserVolumeCommandHandler(
    IUnitOfWork unitOfWork,
    IMediator mediator,
    IUser user,
    CultureLocalizer localizer,
    IRepositoryBase<UserProfile> userProfileRepository
) : IRequestHandler<UpdateUserVolumeCommand, UpdateUserVolumeCommandResponse?>
{
    public async Task<UpdateUserVolumeCommandResponse?> Handle(UpdateUserVolumeCommand command, CancellationToken cancellationToken)
    {
        var volume = Math.Clamp(command.Volume, 0f, 1f);

        var profile = userProfileRepository.Get(x => x.UserId == user.Id);
        if (profile == null)
        {
            await mediator.Publish(new DomainNotification("UpdateUserVolume", localizer.Text("InvalidUser")), cancellationToken);
            return default;
        }

        profile.Volume = volume;
        profile.UpdatedAt = DateTime.UtcNow;
        userProfileRepository.Update(profile);
        unitOfWork.Commit();

        await mediator.Publish(new DomainSuccessNotification("UpdateUserVolume", localizer.Text("Success")), cancellationToken);

        return new UpdateUserVolumeCommandResponse { Volume = profile.Volume };
    }
}
