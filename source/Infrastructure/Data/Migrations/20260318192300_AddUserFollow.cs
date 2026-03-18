using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFollow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_USER_FOLLOW",
                columns: table => new
                {
                    FK_FOLLOWERUSERID = table.Column<Guid>(type: "uuid", nullable: false),
                    FK_FOLLOWEDUSERID = table.Column<Guid>(type: "uuid", nullable: false),
                    DT_CREATEDAT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_USER_FOLLOW", x => new { x.FK_FOLLOWERUSERID, x.FK_FOLLOWEDUSERID });
                    table.ForeignKey(
                        name: "FK_USERFOLLOW_FOLLOWED",
                        column: x => x.FK_FOLLOWEDUSERID,
                        principalTable: "T_USER",
                        principalColumn: "PK_USERID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_USERFOLLOW_FOLLOWER",
                        column: x => x.FK_FOLLOWERUSERID,
                        principalTable: "T_USER",
                        principalColumn: "PK_USERID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_USERFOLLOW_FOLLOWEDUSERID",
                table: "T_USER_FOLLOW",
                column: "FK_FOLLOWEDUSERID");

            migrationBuilder.CreateIndex(
                name: "IX_USERFOLLOW_FOLLOWER_FOLLOWED",
                table: "T_USER_FOLLOW",
                columns: new[] { "FK_FOLLOWERUSERID", "FK_FOLLOWEDUSERID" });

            migrationBuilder.CreateIndex(
                name: "IX_USERFOLLOW_FOLLOWERUSERID",
                table: "T_USER_FOLLOW",
                column: "FK_FOLLOWERUSERID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_USER_FOLLOW");
        }
    }
}
