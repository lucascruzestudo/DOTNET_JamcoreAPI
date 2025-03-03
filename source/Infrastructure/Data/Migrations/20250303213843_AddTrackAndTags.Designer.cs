﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Project.Infrastructure.Data;

#nullable disable

namespace Project.Infrastructure.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250303213843_AddTrackAndTags")]
    partial class AddTrackAndTags
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Project.Domain.Entities.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("PK_ROLEID");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("DT_CREATEDAT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("FL_DELETED");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("TX_NAME");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("DT_UPDATEDAT");

                    b.HasKey("Id");

                    b.ToTable("T_ROLE", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"),
                            CreatedAt = new DateTime(2025, 3, 3, 21, 38, 42, 631, DateTimeKind.Utc).AddTicks(5359),
                            IsDeleted = false,
                            Name = "Admin"
                        },
                        new
                        {
                            Id = new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9b"),
                            CreatedAt = new DateTime(2025, 3, 3, 21, 38, 42, 631, DateTimeKind.Utc).AddTicks(5362),
                            IsDeleted = false,
                            Name = "User"
                        });
                });

            modelBuilder.Entity("Project.Domain.Entities.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("PK_TAGID");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("DT_CREATEDAT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("FL_DELETED");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)")
                        .HasColumnName("TX_NAME");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("DT_UPDATEDAT");

                    b.HasKey("Id");

                    b.ToTable("T_TAG", (string)null);
                });

            modelBuilder.Entity("Project.Domain.Entities.Track", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("PK_TRACKID");

                    b.Property<string>("AudioFileUrl")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("TX_AUDIOFILEURL");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("DT_CREATEDAT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("TX_DESCRIPTION");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("TX_IMAGEURL");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("FL_DELETED");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("boolean")
                        .HasColumnName("FL_ISPUBLIC");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("TX_TITLE");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("DT_UPDATEDAT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("FK_USERID");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("T_TRACK", (string)null);
                });

            modelBuilder.Entity("Project.Domain.Entities.TrackTag", b =>
                {
                    b.Property<Guid>("TrackId")
                        .HasColumnType("uuid")
                        .HasColumnName("FK_TRACKID");

                    b.Property<Guid>("TagId")
                        .HasColumnType("uuid")
                        .HasColumnName("FK_TAGID");

                    b.HasKey("TrackId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("T_TRACK_TAG", (string)null);
                });

            modelBuilder.Entity("Project.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("PK_USERID");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("DT_CREATEDAT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("TX_EMAIL");

                    b.Property<string>("HashedPassword")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("TX_PASSWORD");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("FL_DELETED");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uuid")
                        .HasColumnName("FK_ROLEID");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("DT_UPDATEDAT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("TX_USERNAME");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("T_USER", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("1ebb6855-71db-48e0-9bfb-056cf944ef5d"),
                            CreatedAt = new DateTime(2025, 3, 3, 21, 38, 42, 631, DateTimeKind.Utc).AddTicks(5541),
                            Email = "admin@system.com",
                            HashedPassword = "$2a$11$/vcMOOStstzVnRSm1cZB7.qTjHG1yIwk9DUcpE1Ibth0rWmIbwp.O",
                            IsDeleted = false,
                            RoleId = new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"),
                            Username = "administrator"
                        },
                        new
                        {
                            Id = new Guid("0f9e5907-67f4-4d5d-93a2-d05bf6e817c0"),
                            CreatedAt = new DateTime(2025, 3, 3, 21, 38, 42, 749, DateTimeKind.Utc).AddTicks(2244),
                            Email = "lucascruzestudo@gmail.com",
                            HashedPassword = "$2a$11$IPEt2FHQwYPdpXVYhlwifuxQrT9GMjgjWrWhUwVUDYbImGFxYvYB.",
                            IsDeleted = false,
                            RoleId = new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"),
                            Username = "lucascruzestudo"
                        },
                        new
                        {
                            Id = new Guid("89881702-6a1d-46c3-9258-89e96b0376d3"),
                            CreatedAt = new DateTime(2025, 3, 3, 21, 38, 42, 868, DateTimeKind.Utc).AddTicks(1930),
                            Email = "lucascruztrabalho@gmail.com",
                            HashedPassword = "$2a$11$R8VTPL22rJXqXTV1XrllNOXZ5Nl2qFXsTVrZa8dnZqtu2U/WAVIoO",
                            IsDeleted = false,
                            RoleId = new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9b"),
                            Username = "lucascruztrabalho"
                        });
                });

            modelBuilder.Entity("Project.Domain.Entities.UserProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("PK_USERPROFILEID");

                    b.Property<string>("Bio")
                        .HasColumnType("text")
                        .HasColumnName("TX_BIO");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("DT_CREATEDAT");

                    b.Property<string>("DisplayName")
                        .HasColumnType("text")
                        .HasColumnName("TX_DISPLAYNAME");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("FL_DELETED");

                    b.Property<string>("Location")
                        .HasColumnType("text")
                        .HasColumnName("TX_LOCATION");

                    b.Property<string>("ProfilePictureUrl")
                        .HasColumnType("text")
                        .HasColumnName("TX_PROFILEPICTUREURL");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("DT_UPDATEDAT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("FK_USERID");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("T_USERPROFILE", (string)null);
                });

            modelBuilder.Entity("Project.Domain.Entities.Track", b =>
                {
                    b.HasOne("Project.Domain.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_TRACK_USER");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Project.Domain.Entities.TrackTag", b =>
                {
                    b.HasOne("Project.Domain.Entities.Tag", "Tag")
                        .WithMany("TrackTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_TRACKTAG_TAG");

                    b.HasOne("Project.Domain.Entities.Track", "Track")
                        .WithMany("TrackTags")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_TRACKTAG_TRACK");

                    b.Navigation("Tag");

                    b.Navigation("Track");
                });

            modelBuilder.Entity("Project.Domain.Entities.User", b =>
                {
                    b.HasOne("Project.Domain.Entities.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_USER_ROLE");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Project.Domain.Entities.UserProfile", b =>
                {
                    b.HasOne("Project.Domain.Entities.User", "User")
                        .WithOne("UserProfile")
                        .HasForeignKey("Project.Domain.Entities.UserProfile", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_USERPROFILE_USER");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Project.Domain.Entities.Role", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("Project.Domain.Entities.Tag", b =>
                {
                    b.Navigation("TrackTags");
                });

            modelBuilder.Entity("Project.Domain.Entities.Track", b =>
                {
                    b.Navigation("TrackTags");
                });

            modelBuilder.Entity("Project.Domain.Entities.User", b =>
                {
                    b.Navigation("UserProfile");
                });
#pragma warning restore 612, 618
        }
    }
}
