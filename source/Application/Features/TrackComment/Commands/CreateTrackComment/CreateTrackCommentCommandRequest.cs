namespace Project.Application.Features.Commands.CreateTrackComment;

public record CreateTrackCommentCommandRequest
{
    public Guid TrackId { get; init; }
    public Guid? ParentCommentId { get; init; }
    public string Comment { get; init; } = string.Empty;
}