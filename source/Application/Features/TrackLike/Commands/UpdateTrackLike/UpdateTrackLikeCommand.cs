using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.UpdateTrackLike;

public class UpdateTrackLikeCommand(UpdateTrackLikeCommandRequest request) : Command<UpdateTrackLikeCommandResponse>
{
    public UpdateTrackLikeCommandRequest Request { get; set; } = request;
}
