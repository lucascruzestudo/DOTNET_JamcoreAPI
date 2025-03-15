using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities;

namespace Project.Infrastructure.Data.Configurations
{
       internal class TrackTagConfiguration : IEntityTypeConfiguration<TrackTag>
       {
              public void Configure(EntityTypeBuilder<TrackTag> builder)
              {
                     builder.ToTable("T_TRACK_TAG");

                     builder.HasKey(tt => new { tt.TrackId, tt.TagId });

                     builder.Property(tt => tt.TrackId)
                            .HasColumnName("FK_TRACKID")
                            .IsRequired();

                     builder.Property(tt => tt.TagId)
                            .HasColumnName("FK_TAGID")
                            .IsRequired();

                     builder.HasOne(tt => tt.Track)
                            .WithMany(t => t.TrackTags)
                            .HasForeignKey(tt => tt.TrackId)
                            .HasConstraintName("FK_TRACKTAG_TRACK")
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasOne(tt => tt.Tag)
                            .WithMany(t => t.TrackTags)
                            .HasForeignKey(tt => tt.TagId)
                            .HasConstraintName("FK_TRACKTAG_TAG")
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasIndex(tt => new { tt.TrackId, tt.TagId })
                                 .HasDatabaseName("IX_TRACK_TAGS_TRACKID_TAGID");

                     builder.HasIndex(tt => tt.TrackId)
                            .HasDatabaseName("IX_TRACK_TAGS_TRACKID");

                     builder.HasIndex(tt => tt.TagId)
                            .HasDatabaseName("IX_TRACK_TAGS_TAGID");
              }
       }
}
