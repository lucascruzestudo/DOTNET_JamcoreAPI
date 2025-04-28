namespace Project.Domain.ViewModels;


    public record CommentViewModel
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public Guid TrackId { get; set; }
        public string TrackName { get; set; } = string.Empty;
    }
