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
    [Migration("20250309222043_FixTrackPlayAddNewKey")]
    partial class FixTrackPlayAddNewKey
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

                    b.Property<string>("AudioFileName")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("TX_AUDIOFILENAME");

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

                    b.Property<string>("ImageFileName")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)")
                        .HasColumnName("TX_IMAGEFILENAME");

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

            modelBuilder.Entity("Project.Domain.Entities.TrackComment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("PK_TRACKCOMMENTID");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)")
                        .HasColumnName("TX_COMMENT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("DT_CREATEDAT");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("FL_DELETED");

                    b.Property<Guid?>("ParentCommentId")
                        .HasColumnType("uuid")
                        .HasColumnName("FK_PARENTCOMMENTID");

                    b.Property<Guid>("TrackId")
                        .HasColumnType("uuid")
                        .HasColumnName("FK_TRACKID");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("DT_UPDATEDAT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("FK_USERID");

                    b.HasKey("Id");

                    b.HasIndex("ParentCommentId");

                    b.HasIndex("TrackId");

                    b.HasIndex("UserId");

                    b.ToTable("T_TRACK_COMMENT", (string)null);
                });

            modelBuilder.Entity("Project.Domain.Entities.TrackLike", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("FK_USERID");

                    b.Property<Guid>("TrackId")
                        .HasColumnType("uuid")
                        .HasColumnName("FK_TRACKID");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("DT_CREATEDAT");

                    b.HasKey("UserId", "TrackId");

                    b.HasIndex("TrackId");

                    b.ToTable("T_TRACK_LIKE", (string)null);
                });

            modelBuilder.Entity("Project.Domain.Entities.TrackPlay", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("PK_TRACKPLAYID");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("DT_CREATEDAT");

                    b.Property<Guid>("TrackId")
                        .HasColumnType("uuid")
                        .HasColumnName("FK_TRACKID");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid")
                        .HasColumnName("FK_USERID");

                    b.HasKey("Id");

                    b.HasIndex("TrackId");

                    b.HasIndex("UserId");

                    b.ToTable("T_TRACK_PLAY", (string)null);
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

            modelBuilder.Entity("Project.Domain.Entities.TrackComment", b =>
                {
                    b.HasOne("Project.Domain.Entities.TrackComment", "ParentComment")
                        .WithMany("Replies")
                        .HasForeignKey("ParentCommentId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .HasConstraintName("FK_TRACKCOMMENT_PARENT");

                    b.HasOne("Project.Domain.Entities.Track", "Track")
                        .WithMany("TrackComments")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_TRACKCOMMENT_TRACK");

                    b.HasOne("Project.Domain.Entities.User", "User")
                        .WithMany("UserComments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_TRACKCOMMENT_USER");

                    b.Navigation("ParentComment");

                    b.Navigation("Track");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Project.Domain.Entities.TrackLike", b =>
                {
                    b.HasOne("Project.Domain.Entities.Track", "Track")
                        .WithMany("TrackLikes")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_TRACKLIKE_TRACK");

                    b.HasOne("Project.Domain.Entities.User", "User")
                        .WithMany("LikedTracks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_TRACKLIKE_USER");

                    b.Navigation("Track");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Project.Domain.Entities.TrackPlay", b =>
                {
                    b.HasOne("Project.Domain.Entities.Track", "Track")
                        .WithMany("TrackPlays")
                        .HasForeignKey("TrackId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_TRACKPLAY_TRACK");

                    b.HasOne("Project.Domain.Entities.User", "User")
                        .WithMany("PlayedTracks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_TRACKPLAY_USER");

                    b.Navigation("Track");

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
                    b.Navigation("TrackComments");

                    b.Navigation("TrackLikes");

                    b.Navigation("TrackPlays");

                    b.Navigation("TrackTags");
                });

            modelBuilder.Entity("Project.Domain.Entities.TrackComment", b =>
                {
                    b.Navigation("Replies");
                });

            modelBuilder.Entity("Project.Domain.Entities.User", b =>
                {
                    b.Navigation("LikedTracks");

                    b.Navigation("PlayedTracks");

                    b.Navigation("UserComments");

                    b.Navigation("UserProfile");
                });
#pragma warning restore 612, 618
        }
    }
}
