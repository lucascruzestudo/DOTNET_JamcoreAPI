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
    [Migration("20250302221236_AddUserProfile")]
    partial class AddUserProfile
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
                            CreatedAt = new DateTime(2025, 3, 2, 22, 12, 35, 350, DateTimeKind.Utc).AddTicks(4796),
                            IsDeleted = false,
                            Name = "Admin"
                        },
                        new
                        {
                            Id = new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9b"),
                            CreatedAt = new DateTime(2025, 3, 2, 22, 12, 35, 350, DateTimeKind.Utc).AddTicks(4800),
                            IsDeleted = false,
                            Name = "User"
                        });
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
                            Id = new Guid("4154155f-f934-4cd8-b517-0a1114e3c11d"),
                            CreatedAt = new DateTime(2025, 3, 2, 22, 12, 35, 350, DateTimeKind.Utc).AddTicks(4984),
                            Email = "admin@system.com",
                            HashedPassword = "$2a$11$/GCuRGYpujcCJqx2NFMsu.caO/KR4WaC6HeTKtXu1YpPsKKWYtxF2",
                            IsDeleted = false,
                            RoleId = new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"),
                            Username = "administrator"
                        },
                        new
                        {
                            Id = new Guid("0bba6d29-5b5b-4cc2-8b6a-cfbe8f34e813"),
                            CreatedAt = new DateTime(2025, 3, 2, 22, 12, 35, 465, DateTimeKind.Utc).AddTicks(6856),
                            Email = "lucascruzestudo@gmail.com",
                            HashedPassword = "$2a$11$IIR3V.2fqeMZpjYMIC21L.ruj.NQJM3vxkBjehoW9IRDeXfRy1tOu",
                            IsDeleted = false,
                            RoleId = new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"),
                            Username = "lucascruzestudo"
                        },
                        new
                        {
                            Id = new Guid("24ed49ca-b719-428f-8c58-83ec139cd326"),
                            CreatedAt = new DateTime(2025, 3, 2, 22, 12, 35, 582, DateTimeKind.Utc).AddTicks(7472),
                            Email = "lucascruztrabalho@gmail.com",
                            HashedPassword = "$2a$11$tUUMBJKzvw1OsU1RH6A60O4Mzyp1eBSXuejy1a45Bw320sIfIlGsu",
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

            modelBuilder.Entity("Project.Domain.Entities.User", b =>
                {
                    b.Navigation("UserProfile");
                });
#pragma warning restore 612, 618
        }
    }
}
