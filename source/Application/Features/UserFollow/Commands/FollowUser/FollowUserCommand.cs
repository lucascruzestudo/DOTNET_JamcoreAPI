using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.FollowUser;

public class FollowUserCommand(FollowUserCommandRequest request) : Command<FollowUserCommandResponse>
{
    public FollowUserCommandRequest Request { get; set; } = request;
}
