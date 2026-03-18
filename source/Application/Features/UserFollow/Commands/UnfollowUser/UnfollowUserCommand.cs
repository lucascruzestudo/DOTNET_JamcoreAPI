using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.UnfollowUser;

public class UnfollowUserCommand(UnfollowUserCommandRequest request) : Command<UnfollowUserCommandResponse>
{
    public UnfollowUserCommandRequest Request { get; set; } = request;
}
