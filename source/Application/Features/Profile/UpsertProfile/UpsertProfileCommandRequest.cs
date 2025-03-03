namespace Project.Application.Features.Commands.UpsertProfile;

public record UpsertProfileCommandRequest
{
    public string? DisplayName { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
}