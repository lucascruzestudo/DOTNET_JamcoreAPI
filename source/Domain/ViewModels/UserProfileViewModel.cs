namespace Project.Domain.ViewModels;

public record UserProfileViewModel
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string ProfilePictureUrl { get; set; } = string.Empty;
}