using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities;

namespace Project.Infrastructure.Data.Configurations
{
    internal class UserFollowConfiguration : IEntityTypeConfiguration<UserFollow>
    {
        public void Configure(EntityTypeBuilder<UserFollow> builder)
        {
            builder.ToTable("T_USER_FOLLOW");

            builder.HasKey(uf => new { uf.FollowerUserId, uf.FollowedUserId });

            builder.Property(uf => uf.FollowerUserId)
                .HasColumnName("FK_FOLLOWERUSERID")
                .IsRequired();

            builder.Property(uf => uf.FollowedUserId)
                .HasColumnName("FK_FOLLOWEDUSERID")
                .IsRequired();

            builder.Property(uf => uf.CreatedAt)
                .HasColumnName("DT_CREATEDAT")
                .IsRequired();

            builder.HasOne(uf => uf.Follower)
                .WithMany(u => u.Following)
                .HasForeignKey(uf => uf.FollowerUserId)
                .HasConstraintName("FK_USERFOLLOW_FOLLOWER")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(uf => uf.Followed)
                .WithMany(u => u.Followers)
                .HasForeignKey(uf => uf.FollowedUserId)
                .HasConstraintName("FK_USERFOLLOW_FOLLOWED")
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(uf => uf.FollowerUserId)
                .HasDatabaseName("IX_USERFOLLOW_FOLLOWERUSERID");

            builder.HasIndex(uf => uf.FollowedUserId)
                .HasDatabaseName("IX_USERFOLLOW_FOLLOWEDUSERID");

            builder.HasIndex(uf => new { uf.FollowerUserId, uf.FollowedUserId })
                .HasDatabaseName("IX_USERFOLLOW_FOLLOWER_FOLLOWED");
        }
    }
}
