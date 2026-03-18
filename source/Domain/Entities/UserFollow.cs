namespace Project.Domain.Entities
{
    public class UserFollow
    {
        public Guid FollowerUserId { get; set; }
        public virtual User? Follower { get; set; }

        public Guid FollowedUserId { get; set; }
        public virtual User? Followed { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
