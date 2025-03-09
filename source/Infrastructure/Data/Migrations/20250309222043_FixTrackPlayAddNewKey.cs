using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixTrackPlayAddNewKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_T_TRACK_PLAY",
                table: "T_TRACK_PLAY");

            migrationBuilder.AddColumn<Guid>(
                name: "PK_TRACKPLAYID",
                table: "T_TRACK_PLAY",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_TRACK_PLAY",
                table: "T_TRACK_PLAY",
                column: "PK_TRACKPLAYID");

            migrationBuilder.CreateIndex(
                name: "IX_T_TRACK_PLAY_FK_USERID",
                table: "T_TRACK_PLAY",
                column: "FK_USERID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_T_TRACK_PLAY",
                table: "T_TRACK_PLAY");

            migrationBuilder.DropIndex(
                name: "IX_T_TRACK_PLAY_FK_USERID",
                table: "T_TRACK_PLAY");

            migrationBuilder.DropColumn(
                name: "PK_TRACKPLAYID",
                table: "T_TRACK_PLAY");

            migrationBuilder.AddPrimaryKey(
                name: "PK_T_TRACK_PLAY",
                table: "T_TRACK_PLAY",
                columns: new[] { "FK_USERID", "FK_TRACKID" });
        }
    }
}
