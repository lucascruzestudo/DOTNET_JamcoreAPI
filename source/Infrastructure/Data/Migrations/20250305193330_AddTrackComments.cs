using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_TRACK_COMMENT",
                columns: table => new
                {
                    PK_TRACKCOMMENTID = table.Column<Guid>(type: "uuid", nullable: false),
                    TX_COMMENT = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FK_TRACKID = table.Column<Guid>(type: "uuid", nullable: false),
                    FK_USERID = table.Column<Guid>(type: "uuid", nullable: false),
                    FK_PARENTCOMMENTID = table.Column<Guid>(type: "uuid", nullable: true),
                    DT_CREATEDAT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DT_UPDATEDAT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FL_DELETED = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_TRACK_COMMENT", x => x.PK_TRACKCOMMENTID);
                    table.ForeignKey(
                        name: "FK_TRACKCOMMENT_PARENT",
                        column: x => x.FK_PARENTCOMMENTID,
                        principalTable: "T_TRACK_COMMENT",
                        principalColumn: "PK_TRACKCOMMENTID");
                    table.ForeignKey(
                        name: "FK_TRACKCOMMENT_TRACK",
                        column: x => x.FK_TRACKID,
                        principalTable: "T_TRACK",
                        principalColumn: "PK_TRACKID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TRACKCOMMENT_USER",
                        column: x => x.FK_USERID,
                        principalTable: "T_USER",
                        principalColumn: "PK_USERID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_TRACK_COMMENT_FK_PARENTCOMMENTID",
                table: "T_TRACK_COMMENT",
                column: "FK_PARENTCOMMENTID");

            migrationBuilder.CreateIndex(
                name: "IX_T_TRACK_COMMENT_FK_TRACKID",
                table: "T_TRACK_COMMENT",
                column: "FK_TRACKID");

            migrationBuilder.CreateIndex(
                name: "IX_T_TRACK_COMMENT_FK_USERID",
                table: "T_TRACK_COMMENT",
                column: "FK_USERID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_TRACK_COMMENT");
        }
    }
}
