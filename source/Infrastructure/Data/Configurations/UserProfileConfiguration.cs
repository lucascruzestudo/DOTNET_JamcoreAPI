using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities;

namespace Project.Infrastructure.Data.Configurations
{
    internal class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("T_USERPROFILE");

            builder.HasKey(up => up.Id);

            builder.Property(x => x.Id)
                   .HasColumnType("uuid")
                   .HasColumnName("PK_USERPROFILEID")
                   .ValueGeneratedOnAdd();

            builder.Property(up => up.UserId)
                   .HasColumnName("FK_USERID")
                   .IsRequired();

            builder.Property(up => up.DisplayName)
                   .HasColumnName("TX_DISPLAYNAME");

            builder.Property(up => up.Bio)
                   .HasColumnName("TX_BIO");

            builder.Property(up => up.Location)
                   .HasColumnName("TX_LOCATION");

            builder.Property(up => up.ProfilePictureUrl)
                   .HasColumnName("TX_PROFILEPICTUREURL");

            builder.Property(up => up.Volume)
                   .HasColumnName("NM_VOLUME")
                   .HasDefaultValue(1.0f)
                   .IsRequired();

            builder.HasOne(up => up.User)
                   .WithOne(u => u.UserProfile)
                   .HasForeignKey<UserProfile>(up => up.UserId)
                   .HasConstraintName("FK_USERPROFILE_USER");

            builder.Property(up => up.CreatedAt)
                   .HasColumnName("DT_CREATEDAT")
                   .IsRequired();

            builder.Property(up => up.UpdatedAt)
                   .HasColumnName("DT_UPDATEDAT");

            builder.Property(up => up.IsDeleted)
                   .HasColumnName("FL_DELETED")
                   .IsRequired();

            // 1-to-1: every query joins UserProfile by UserId
            builder.HasIndex(up => up.UserId)
                   .IsUnique()
                   .HasDatabaseName("IX_USERPROFILE_USERID");

            // SearchTracks: DisplayName.Contains(term)
            builder.HasIndex(up => up.DisplayName)
                   .HasDatabaseName("IX_USERPROFILE_DISPLAYNAME");
        }
    }
}
