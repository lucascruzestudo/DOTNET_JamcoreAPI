namespace Project.Application.Features.Commands.CreateUserProfileComment;

public record CreateUserProfileCommentCommandRequest
{
    public Guid UserProfileId { get; init; }
    public string Comment { get; init; } = string.Empty;
}
