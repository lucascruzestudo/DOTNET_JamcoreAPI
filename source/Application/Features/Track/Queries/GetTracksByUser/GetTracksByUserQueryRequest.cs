namespace Project.Application.Features.Commands.GetTracksByUser;

public record GetTracksByUserQueryRequest
{
    public string Tag { get; set; } = string.Empty;
}