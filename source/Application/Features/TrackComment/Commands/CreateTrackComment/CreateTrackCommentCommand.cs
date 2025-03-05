using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.CreateTrackComment;

public class CreateTrackCommentCommand(CreateTrackCommentCommandRequest request) : Command<CreateTrackCommentCommandResponse>
{
    public CreateTrackCommentCommandRequest Request { get; set; } = request;
}
