using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTrackPlayCountIntoNewEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NM_PLAYCOUNT",
                table: "T_TRACK");

            migrationBuilder.CreateTable(
                name: "T_TRACK_PLAY",
                columns: table => new
                {
                    FK_USERID = table.Column<Guid>(type: "uuid", nullable: false),
                    FK_TRACKID = table.Column<Guid>(type: "uuid", nullable: false),
                    DT_CREATEDAT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_TRACK_PLAY", x => new { x.FK_USERID, x.FK_TRACKID });
                    table.ForeignKey(
                        name: "FK_TRACKPLAY_TRACK",
                        column: x => x.FK_TRACKID,
                        principalTable: "T_TRACK",
                        principalColumn: "PK_TRACKID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TRACKPLAY_USER",
                        column: x => x.FK_USERID,
                        principalTable: "T_USER",
                        principalColumn: "PK_USERID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_TRACK_PLAY_FK_TRACKID",
                table: "T_TRACK_PLAY",
                column: "FK_TRACKID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_TRACK_PLAY");

            migrationBuilder.AddColumn<int>(
                name: "NM_PLAYCOUNT",
                table: "T_TRACK",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
