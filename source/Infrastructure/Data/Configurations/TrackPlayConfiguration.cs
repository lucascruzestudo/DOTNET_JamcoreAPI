using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities;

namespace Project.Infrastructure.Data.Configurations
{
       internal class TrackPlayConfiguration : IEntityTypeConfiguration<TrackPlay>
       {
              public void Configure(EntityTypeBuilder<TrackPlay> builder)
              {
                     builder.ToTable("T_TRACK_PLAY");

                     builder.HasKey(t => t.Id);

                     builder.Property(t => t.Id)
                                     .HasColumnType("uuid")
                                     .HasColumnName("PK_TRACKPLAYID")
                                     .ValueGeneratedOnAdd();

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
                            .WithMany(u => u.PlayedTracks)
                            .HasForeignKey(tl => tl.UserId)
                            .HasConstraintName("FK_TRACKPLAY_USER")
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasOne(tl => tl.Track)
                            .WithMany(t => t.TrackPlays)
                            .HasForeignKey(tl => tl.TrackId)
                            .HasConstraintName("FK_TRACKPLAY_TRACK")
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasIndex(tl => new { tl.UserId, tl.TrackId })
                                 .HasDatabaseName("IX_TRACKPLAY_USERID_TRACKID");

                     builder.HasIndex(tl => tl.UserId)
                            .HasDatabaseName("IX_TRACKPLAY_USERID");

                     builder.HasIndex(tl => tl.TrackId)
                            .HasDatabaseName("IX_TRACKPLAY_TRACKID");

                     builder.HasIndex(tl => tl.CreatedAt)
                            .HasDatabaseName("IX_TRACKPLAY_CREATEDAT");
              }
       }
}