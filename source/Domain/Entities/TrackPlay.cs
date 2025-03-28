namespace Project.Domain.Entities
{
    public class TrackPlay
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        
        public Guid TrackId { get; set; }
        public virtual Track? Track { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
