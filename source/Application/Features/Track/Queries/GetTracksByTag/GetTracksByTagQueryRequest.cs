namespace Project.Application.Features.Commands.GetTracksByTag;

public record GetTracksByTagQueryRequest
{
    public string Tag { get; set; } = string.Empty;
}