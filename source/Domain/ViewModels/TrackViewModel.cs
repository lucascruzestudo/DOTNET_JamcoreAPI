namespace Project.Domain.ViewModels;

public record TrackViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string[] Tags { get; set; } = [];
    public string AudioFileUrl { get; set; } = string.Empty;
    public string AudioFileName { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string ImageFileName { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int LikeCount { get; set; }
    public int PlayCount { get; set; }
}