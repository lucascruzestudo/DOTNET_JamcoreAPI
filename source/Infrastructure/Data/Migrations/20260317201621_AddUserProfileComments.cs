using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfileComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_USERPROFILE_COMMENT",
                columns: table => new
                {
                    PK_USERPROFILECOMMENTID = table.Column<Guid>(type: "uuid", nullable: false),
                    TX_COMMENT = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FK_USERPROFILEID = table.Column<Guid>(type: "uuid", nullable: false),
                    FK_USERID = table.Column<Guid>(type: "uuid", nullable: false),
                    DT_CREATEDAT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DT_UPDATEDAT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FL_DELETED = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_USERPROFILE_COMMENT", x => x.PK_USERPROFILECOMMENTID);
                    table.ForeignKey(
                        name: "FK_USERPROFILECOMMENT_USER",
                        column: x => x.FK_USERID,
                        principalTable: "T_USER",
                        principalColumn: "PK_USERID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_USERPROFILECOMMENT_USERPROFILE",
                        column: x => x.FK_USERPROFILEID,
                        principalTable: "T_USERPROFILE",
                        principalColumn: "PK_USERPROFILEID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_USERPROFILECOMMENT_CREATEDAT",
                table: "T_USERPROFILE_COMMENT",
                column: "DT_CREATEDAT");

            migrationBuilder.CreateIndex(
                name: "IX_USERPROFILECOMMENT_USERID",
                table: "T_USERPROFILE_COMMENT",
                column: "FK_USERID");

            migrationBuilder.CreateIndex(
                name: "IX_USERPROFILECOMMENT_USERPROFILEID",
                table: "T_USERPROFILE_COMMENT",
                column: "FK_USERPROFILEID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_USERPROFILE_COMMENT");
        }
    }
}
