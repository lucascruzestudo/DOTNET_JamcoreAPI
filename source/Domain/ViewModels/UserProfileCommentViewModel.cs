namespace Project.Domain.ViewModels;

public record UserProfileCommentViewModel
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public Guid UserProfileId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? UserProfilePictureUrl { get; set; }
    public DateTime? UserProfileUpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
