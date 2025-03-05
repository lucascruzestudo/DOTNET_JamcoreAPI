using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Project.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "T_USER",
                keyColumn: "PK_USERID",
                keyValue: new Guid("1a6d842c-e8de-4cdb-b71b-fdd3a2e5e11e"));

            migrationBuilder.DeleteData(
                table: "T_USER",
                keyColumn: "PK_USERID",
                keyValue: new Guid("47f4cd5b-639b-40ba-b6ea-44d19db7f75f"));

            migrationBuilder.DeleteData(
                table: "T_USER",
                keyColumn: "PK_USERID",
                keyValue: new Guid("633c0ceb-a1a6-40fc-beb6-c2d5884015be"));

            migrationBuilder.DeleteData(
                table: "T_ROLE",
                keyColumn: "PK_ROLEID",
                keyValue: new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"));

            migrationBuilder.DeleteData(
                table: "T_ROLE",
                keyColumn: "PK_ROLEID",
                keyValue: new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9b"));

            migrationBuilder.CreateTable(
                name: "T_TRACK_LIKE",
                columns: table => new
                {
                    FK_USERID = table.Column<Guid>(type: "uuid", nullable: false),
                    FK_TRACKID = table.Column<Guid>(type: "uuid", nullable: false),
                    DT_CREATEDAT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_TRACK_LIKE", x => new { x.FK_USERID, x.FK_TRACKID });
                    table.ForeignKey(
                        name: "FK_TRACKLIKE_TRACK",
                        column: x => x.FK_TRACKID,
                        principalTable: "T_TRACK",
                        principalColumn: "PK_TRACKID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TRACKLIKE_USER",
                        column: x => x.FK_USERID,
                        principalTable: "T_USER",
                        principalColumn: "PK_USERID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_TRACK_LIKE_FK_TRACKID",
                table: "T_TRACK_LIKE",
                column: "FK_TRACKID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_TRACK_LIKE");

            migrationBuilder.InsertData(
                table: "T_ROLE",
                columns: new[] { "PK_ROLEID", "DT_CREATEDAT", "FL_DELETED", "TX_NAME", "DT_UPDATEDAT" },
                values: new object[,]
                {
                    { new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"), new DateTime(2025, 3, 4, 13, 34, 12, 272, DateTimeKind.Utc).AddTicks(5497), false, "Admin", null },
                    { new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9b"), new DateTime(2025, 3, 4, 13, 34, 12, 272, DateTimeKind.Utc).AddTicks(5502), false, "User", null }
                });

            migrationBuilder.InsertData(
                table: "T_USER",
                columns: new[] { "PK_USERID", "DT_CREATEDAT", "TX_EMAIL", "TX_PASSWORD", "FL_DELETED", "FK_ROLEID", "DT_UPDATEDAT", "TX_USERNAME" },
                values: new object[,]
                {
                    { new Guid("1a6d842c-e8de-4cdb-b71b-fdd3a2e5e11e"), new DateTime(2025, 3, 4, 13, 34, 12, 272, DateTimeKind.Utc).AddTicks(5712), "admin@system.com", "$2a$11$ImKmJVYLLw3HmJUty2R5ZOaR4J2EcZXrs5wVNg19QnkX40iREbfr2", false, new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"), null, "administrator" },
                    { new Guid("47f4cd5b-639b-40ba-b6ea-44d19db7f75f"), new DateTime(2025, 3, 4, 13, 34, 12, 504, DateTimeKind.Utc).AddTicks(6229), "lucascruztrabalho@gmail.com", "$2a$11$oz3mCoccWuoPv3isAOHJEOtZOYae9mzD246xhEepmKRjjOS1VdGsS", false, new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9b"), null, "lucascruztrabalho" },
                    { new Guid("633c0ceb-a1a6-40fc-beb6-c2d5884015be"), new DateTime(2025, 3, 4, 13, 34, 12, 391, DateTimeKind.Utc).AddTicks(98), "lucascruzestudo@gmail.com", "$2a$11$dW11E1xiU4bJX8IBNMq2Aea35WjvcK7EDbHCT0wYpx/AhUzzrB.Vi", false, new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"), null, "lucascruzestudo" }
                });
        }
    }
}
