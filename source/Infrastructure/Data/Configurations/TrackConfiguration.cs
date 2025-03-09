using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities;

namespace Project.Infrastructure.Data.Configurations
{
       internal class TrackConfiguration : IEntityTypeConfiguration<Track>
       {
              public void Configure(EntityTypeBuilder<Track> builder)
              {
                     builder.ToTable("T_TRACK");

                     builder.HasKey(t => t.Id);

                     builder.Property(t => t.Id)
                            .HasColumnType("uuid")
                            .HasColumnName("PK_TRACKID")
                            .ValueGeneratedOnAdd();

                     builder.Property(t => t.Title)
                            .HasColumnName("TX_TITLE")
                            .IsRequired()
                            .HasMaxLength(255);

                     builder.Property(t => t.Description)
                            .HasColumnName("TX_DESCRIPTION")
                            .HasMaxLength(1000);

                     builder.Property(t => t.AudioFileUrl)
                            .HasColumnName("TX_AUDIOFILEURL")
                            .IsRequired()
                            .HasMaxLength(500);

                     builder.Property(t => t.AudioFileName)
                            .HasColumnName("TX_AUDIOFILENAME")
                            .IsRequired()
                            .HasMaxLength(500);

                     builder.Property(t => t.ImageUrl)
                            .HasColumnName("TX_IMAGEURL")
                            .IsRequired()
                            .HasMaxLength(500);
                     
                     builder.Property(t => t.ImageFileName)
                            .HasColumnName("TX_IMAGEFILENAME")
                            .IsRequired()
                            .HasMaxLength(500);

                     builder.Property(t => t.IsPublic)
                            .HasColumnName("FL_ISPUBLIC")
                            .IsRequired();

                     builder.Property(t => t.CreatedAt)
                         .HasColumnName("DT_CREATEDAT")
                         .IsRequired();

                     builder.Property(t => t.UpdatedAt)
                             .HasColumnName("DT_UPDATEDAT");

                     builder.Property(t => t.IsDeleted)
                             .HasColumnName("FL_DELETED")
                             .IsRequired();

                     builder.Property(t => t.PlayCount)
                            .HasColumnName("NM_PLAYCOUNT")
                            .IsRequired();

                     builder.Property(t => t.UserId)
                            .HasColumnName("FK_USERID")
                            .IsRequired();

                     builder.HasOne(t => t.User)
                            .WithMany()
                            .HasForeignKey(t => t.UserId)
                            .HasConstraintName("FK_TRACK_USER")
                            .OnDelete(DeleteBehavior.Cascade);
              }
       }
}
