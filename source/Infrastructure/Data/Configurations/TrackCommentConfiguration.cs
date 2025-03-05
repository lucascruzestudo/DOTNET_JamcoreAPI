using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities;

namespace Project.Infrastructure.Data.Configurations
{
       internal class TrackCommentConfiguration : IEntityTypeConfiguration<TrackComment>
       {
              public void Configure(EntityTypeBuilder<TrackComment> builder)
              {
                     builder.ToTable("T_TRACK_COMMENT");

                     builder.HasKey(tc => tc.Id);

                     builder.Property(tc => tc.Id)
                            .HasColumnType("uuid")
                            .HasColumnName("PK_TRACKCOMMENTID")
                            .ValueGeneratedOnAdd();

                     builder.Property(tc => tc.Comment)
                            .HasColumnName("TX_COMMENT")
                            .IsRequired()
                            .HasMaxLength(1000);

                     builder.Property(tc => tc.UserId)
                            .HasColumnName("FK_USERID")
                            .IsRequired();

                     builder.Property(tc => tc.TrackId)
                            .HasColumnName("FK_TRACKID")
                            .IsRequired();

                     builder.Property(tc => tc.ParentCommentId)
                            .HasColumnName("FK_PARENTCOMMENTID")
                            .IsRequired(false);

                     builder.Property(tc => tc.CreatedAt)
                            .HasColumnName("DT_CREATEDAT")
                            .IsRequired();

                     builder.Property(up => up.CreatedAt)
                     .HasColumnName("DT_CREATEDAT")
                     .IsRequired();

                     builder.Property(up => up.UpdatedAt)
                            .HasColumnName("DT_UPDATEDAT");

                     builder.Property(up => up.IsDeleted)
                            .HasColumnName("FL_DELETED")
                            .IsRequired();

                     builder.HasOne(tc => tc.User)
                            .WithMany(u => u.UserComments)
                            .HasForeignKey(tc => tc.UserId)
                            .HasConstraintName("FK_TRACKCOMMENT_USER")
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasOne(tc => tc.Track)
                            .WithMany(t => t.TrackComments)
                            .HasForeignKey(tc => tc.TrackId)
                            .HasConstraintName("FK_TRACKCOMMENT_TRACK")
                            .OnDelete(DeleteBehavior.Cascade);

                     builder.HasOne(tc => tc.ParentComment)
                            .WithMany(tc => tc.Replies)
                            .HasForeignKey(tc => tc.ParentCommentId)
                            .HasConstraintName("FK_TRACKCOMMENT_PARENT")
                            .OnDelete(DeleteBehavior.NoAction);
              }
       }
}
