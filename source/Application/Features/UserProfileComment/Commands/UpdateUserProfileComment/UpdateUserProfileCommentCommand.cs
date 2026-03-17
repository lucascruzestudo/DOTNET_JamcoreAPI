using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.UpdateUserProfileComment;

public class UpdateUserProfileCommentCommand(UpdateUserProfileCommentCommandRequest request) : Command<UpdateUserProfileCommentCommandResponse>
{
    public UpdateUserProfileCommentCommandRequest Request { get; set; } = request;
}
