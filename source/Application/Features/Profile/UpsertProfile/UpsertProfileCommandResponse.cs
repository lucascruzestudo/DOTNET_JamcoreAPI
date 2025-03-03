namespace Project.Application.Features.Commands.UpsertProfile;

public record UpsertProfileCommandResponse
{
    public required string DisplayName { get; set; }
    public required string Bio { get; set; }
    public required string Location { get; set; }
    public required string ProfilePictureUrl { get; set; }
}