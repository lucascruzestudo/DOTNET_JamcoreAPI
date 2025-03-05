namespace Project.Application.Features.Commands.DeleteTrackComment;

public record DeleteTrackCommentCommandRequest
{
    public Guid TrackId { get; init; }
    public Guid CommentId { get; init; }

}