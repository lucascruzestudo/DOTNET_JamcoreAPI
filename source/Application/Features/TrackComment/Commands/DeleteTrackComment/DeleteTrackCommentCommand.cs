using Project.Domain.Notifications;

namespace Project.Application.Features.Commands.DeleteTrackComment;

public class DeleteTrackCommentCommand(DeleteTrackCommentCommandRequest request) : Command<DeleteTrackCommentCommandResponse>, IIdempotentCommand
{
    public DeleteTrackCommentCommandRequest Request { get; set; } = request;
}
