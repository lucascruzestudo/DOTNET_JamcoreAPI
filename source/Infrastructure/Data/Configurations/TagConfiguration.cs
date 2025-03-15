using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities;

namespace Project.Infrastructure.Data.Configurations
{
       internal class TagConfiguration : IEntityTypeConfiguration<Tag>
       {
              public void Configure(EntityTypeBuilder<Tag> builder)
              {
                     builder.ToTable("T_TAG");

                     builder.HasKey(t => t.Id);

                     builder.Property(t => t.Id)
                            .HasColumnType("uuid")
                            .HasColumnName("PK_TAGID")
                            .ValueGeneratedOnAdd();

                     builder.Property(t => t.Name)
                            .HasColumnName("TX_NAME")
                            .IsRequired()
                            .HasMaxLength(100);

                     builder.Property(t => t.CreatedAt)
                            .HasColumnName("DT_CREATEDAT")
                            .IsRequired();

                     builder.Property(t => t.UpdatedAt)
                            .HasColumnName("DT_UPDATEDAT");

                     builder.Property(t => t.IsDeleted)
                            .HasColumnName("FL_DELETED")
                            .IsRequired();

                     builder.HasIndex(t => t.Name)
                                 .HasDatabaseName("IX_TAG_NAME");

                     builder.HasIndex(t => t.CreatedAt)
                            .HasDatabaseName("IX_TAG_CREATEDAT");

                     builder.HasIndex(t => t.IsDeleted)
                            .HasDatabaseName("IX_TAG_ISDELETED");
              }
       }
}
