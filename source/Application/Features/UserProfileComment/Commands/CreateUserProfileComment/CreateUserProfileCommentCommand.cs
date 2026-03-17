using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.CreateUserProfileComment;

public class CreateUserProfileCommentCommand(CreateUserProfileCommentCommandRequest request) : Command<CreateUserProfileCommentCommandResponse>
{
    public CreateUserProfileCommentCommandRequest Request { get; set; } = request;
}
