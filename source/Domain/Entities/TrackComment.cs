namespace Project.Domain.Entities
{
    public class TrackComment : BaseEntity
    {
        public string Comment { get; set; } = string.Empty;
        public Guid TrackId { get; set; }
        public virtual Track? Track { get; set; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        public Guid? ParentCommentId { get; set; }
        public virtual TrackComment? ParentComment { get; set; }
        public virtual ICollection<TrackComment> Replies { get; set; } = [];
    }
}
