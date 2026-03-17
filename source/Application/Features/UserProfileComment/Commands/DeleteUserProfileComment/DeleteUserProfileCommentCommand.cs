using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.DeleteUserProfileComment;

public class DeleteUserProfileCommentCommand(DeleteUserProfileCommentCommandRequest request) : Command<DeleteUserProfileCommentCommandResponse>
{
    public DeleteUserProfileCommentCommandRequest Request { get; set; } = request;
}
