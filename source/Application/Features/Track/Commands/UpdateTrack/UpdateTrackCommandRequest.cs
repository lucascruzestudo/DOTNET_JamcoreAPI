namespace Project.Application.Features.Commands.UpdateTrack;

public record UpdateTrackCommandRequest
{
    public Guid TrackId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? IsPublic { get; set; }
    public string[] Tags { get; set; } = [];
}
