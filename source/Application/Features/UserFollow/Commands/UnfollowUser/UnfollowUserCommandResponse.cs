namespace Project.Application.Features.Commands.UnfollowUser;

public record UnfollowUserCommandResponse
{
    public bool IsFollowing { get; set; }
}
