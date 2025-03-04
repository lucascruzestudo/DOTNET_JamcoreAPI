namespace Project.Application.Features.Commands.UpdateTrack;

public record UpdateTrackCommandResponse
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string AudioFileUrl { get; set; } = string.Empty;
}