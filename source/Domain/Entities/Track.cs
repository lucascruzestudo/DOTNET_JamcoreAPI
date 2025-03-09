namespace Project.Domain.Entities
{
    public class Track : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string AudioFileUrl { get; set; } = string.Empty;
        public string AudioFileName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ImageFileName { get; set; } = string.Empty;
        public bool IsPublic { get; set; } = true;
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<TrackTag> TrackTags { get; set; } = [];
        public virtual ICollection<TrackLike> TrackLikes { get; set; } = [];
        public virtual ICollection<TrackPlay> TrackPlays { get; set; } = [];
        public virtual ICollection<TrackComment> TrackComments { get; set; } = [];
    }
}