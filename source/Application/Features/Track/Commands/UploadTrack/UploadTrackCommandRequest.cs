namespace Project.Application.Features.Commands.UploadTrack;

public record UploadTrackCommandRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
    public string[] Tags { get; set; } = [];
}
