namespace Project.Domain.Entities
{
    public class UserProfileComment : BaseEntity
    {
        public string Comment { get; set; } = string.Empty;

        // Target profile receiving the comment
        public Guid UserProfileId { get; set; }
        public virtual UserProfile? UserProfile { get; set; }

        // Author of the comment
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
