using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities;

namespace Project.Infrastructure.Data.Configurations
{
    internal class UserProfileCommentConfiguration : IEntityTypeConfiguration<UserProfileComment>
    {
        public void Configure(EntityTypeBuilder<UserProfileComment> builder)
        {
            builder.ToTable("T_USERPROFILE_COMMENT");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnType("uuid")
                .HasColumnName("PK_USERPROFILECOMMENTID")
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Comment)
                .HasColumnName("TX_COMMENT")
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(x => x.UserProfileId)
                .HasColumnName("FK_USERPROFILEID")
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("FK_USERID")
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .HasColumnName("DT_CREATEDAT")
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .HasColumnName("DT_UPDATEDAT");

            builder.Property(x => x.IsDeleted)
                .HasColumnName("FL_DELETED")
                .IsRequired();

            builder.HasOne(x => x.UserProfile)
                .WithMany(x => x.UserProfileComments)
                .HasForeignKey(x => x.UserProfileId)
                .HasConstraintName("FK_USERPROFILECOMMENT_USERPROFILE")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithMany(x => x.UserProfileComments)
                .HasForeignKey(x => x.UserId)
                .HasConstraintName("FK_USERPROFILECOMMENT_USER")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.UserProfileId)
                .HasDatabaseName("IX_USERPROFILECOMMENT_USERPROFILEID");

            builder.HasIndex(x => x.UserId)
                .HasDatabaseName("IX_USERPROFILECOMMENT_USERID");

            builder.HasIndex(x => x.CreatedAt)
                .HasDatabaseName("IX_USERPROFILECOMMENT_CREATEDAT");
        }
    }
}
