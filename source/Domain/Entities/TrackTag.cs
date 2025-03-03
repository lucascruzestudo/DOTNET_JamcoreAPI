namespace Project.Domain.Entities
{
    public class TrackTag
    {
        public Guid TrackId { get; set; }
        public virtual Track? Track { get; set; }
        public Guid TagId { get; set; }
        public virtual Tag? Tag { get; set; }
    }
}