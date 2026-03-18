namespace Project.Domain.ViewModels;

public record UserProfileViewModel
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
    public float Volume { get; set; } = 1.0f;
    public DateTime UpdatedAt { get; set; }
    public int FollowerCount { get; set; }
    public int FollowingCount { get; set; }
}