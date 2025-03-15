using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities;

namespace Project.Infrastructure.Data.Configurations
{
       internal class TrackLikeConfiguration : IEntityTypeConfiguration<TrackLike>
       {
              public void Configure(EntityTypeBuilder<TrackLike> builder)
              {
                     builder.ToTable("T_TRACK_LIKE");

                     builder.HasKey(tl => new { tl.UserId, tl.TrackId });

                     builder.Property(tl => tl.UserId)
                            .HasColumnName("FK_USERID")
                            .IsRequired();

                     builder.Property(tl => tl.TrackId)
                            .HasColumnName("FK_TRACKID")
                            .IsRequired();

                     builder.Property(t => t.CreatedAt)
                                  .HasColumnName("DT_CREATEDAT")
                                  .IsRequired();

                     builder.HasOne(tl => tl.User)
                            .WithMany(u => u.LikedTracks)
                            .HasForeignKey(tl => tl.UserId)
                            .HasConstraintName("FK_TRACKLIKE_USER")
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasOne(tl => tl.Track)
                            .WithMany(t => t.TrackLikes)
                            .HasForeignKey(tl => tl.TrackId)
                            .HasConstraintName("FK_TRACKLIKE_TRACK")
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasIndex(tl => tl.UserId)
                     .HasDatabaseName("IX_TrackLike_UserId");

                     builder.HasIndex(tl => tl.TrackId)
                            .HasDatabaseName("IX_TRACKLIKE_TRACKID");

                     builder.HasIndex(tl => new { tl.UserId, tl.TrackId })
                            .HasDatabaseName("IX_TRACKLIKE_USERID_TRACKID");
              }
       }
}
