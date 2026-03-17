namespace Project.Application.Features.Commands.DeleteUserProfileComment;

public record DeleteUserProfileCommentCommandRequest
{
    public Guid CommentId { get; init; }
}
