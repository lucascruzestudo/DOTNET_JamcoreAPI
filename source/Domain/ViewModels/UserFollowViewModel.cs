namespace Project.Domain.ViewModels;

public record UserFollowViewModel
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public DateTime? ProfilePictureUpdatedAt { get; set; }
    public DateTime FollowedAt { get; set; }
    public int FollowerCount { get; set; }
}
