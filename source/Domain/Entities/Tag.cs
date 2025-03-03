namespace Project.Domain.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<TrackTag> TrackTags { get; set; } = [];
    }
}
