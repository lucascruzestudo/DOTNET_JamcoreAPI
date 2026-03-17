namespace Project.Application.Features.Commands.UpdateUserProfileComment;

public record UpdateUserProfileCommentCommandRequest
{
    public Guid CommentId { get; init; }
    public string Comment { get; init; } = string.Empty;
}
