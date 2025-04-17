namespace Project.Domain.Entities
{
    public class UserProfile : BaseEntity
    {
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }

        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public string? Location { get; set; }
        public string? ProfilePictureUrl { get; set; }


        public UserProfile(Guid userId, string? displayName, string? bio, string? location, string? profilePictureUrl)
        {
            UserId = userId;
            DisplayName = displayName;
            Bio = bio;
            Location = location;
            ProfilePictureUrl = profilePictureUrl;
        }
        public UserProfile(Guid userId, string? displayName, string? bio, string? location)
        {
            UserId = userId;
            DisplayName = displayName;
            Bio = bio;
            Location = location;
            ProfilePictureUrl = null;
        }

        public UserProfile(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
        }
    }
}