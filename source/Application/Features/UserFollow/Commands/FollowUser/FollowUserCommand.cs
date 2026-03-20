using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.FollowUser;

public class FollowUserCommand(FollowUserCommandRequest request) : Command<FollowUserCommandResponse>, IIdempotentCommand
{
    public FollowUserCommandRequest Request { get; set; } = request;
}
