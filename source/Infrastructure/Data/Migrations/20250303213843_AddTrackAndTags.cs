using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Project.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackAndTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "T_USER",
                keyColumn: "PK_USERID",
                keyValue: new Guid("0bba6d29-5b5b-4cc2-8b6a-cfbe8f34e813"));

            migrationBuilder.DeleteData(
                table: "T_USER",
                keyColumn: "PK_USERID",
                keyValue: new Guid("24ed49ca-b719-428f-8c58-83ec139cd326"));

            migrationBuilder.DeleteData(
                table: "T_USER",
                keyColumn: "PK_USERID",
                keyValue: new Guid("4154155f-f934-4cd8-b517-0a1114e3c11d"));

            migrationBuilder.CreateTable(
                name: "T_TAG",
                columns: table => new
                {
                    PK_TAGID = table.Column<Guid>(type: "uuid", nullable: false),
                    TX_NAME = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DT_CREATEDAT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DT_UPDATEDAT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FL_DELETED = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_TAG", x => x.PK_TAGID);
                });

            migrationBuilder.CreateTable(
                name: "T_TRACK",
                columns: table => new
                {
                    PK_TRACKID = table.Column<Guid>(type: "uuid", nullable: false),
                    TX_TITLE = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    TX_DESCRIPTION = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    TX_AUDIOFILEURL = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    TX_IMAGEURL = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FL_ISPUBLIC = table.Column<bool>(type: "boolean", nullable: false),
                    FK_USERID = table.Column<Guid>(type: "uuid", nullable: false),
                    DT_CREATEDAT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DT_UPDATEDAT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FL_DELETED = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_TRACK", x => x.PK_TRACKID);
                    table.ForeignKey(
                        name: "FK_TRACK_USER",
                        column: x => x.FK_USERID,
                        principalTable: "T_USER",
                        principalColumn: "PK_USERID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "T_TRACK_TAG",
                columns: table => new
                {
                    FK_TRACKID = table.Column<Guid>(type: "uuid", nullable: false),
                    FK_TAGID = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_TRACK_TAG", x => new { x.FK_TRACKID, x.FK_TAGID });
                    table.ForeignKey(
                        name: "FK_TRACKTAG_TAG",
                        column: x => x.FK_TAGID,
                        principalTable: "T_TAG",
                        principalColumn: "PK_TAGID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TRACKTAG_TRACK",
                        column: x => x.FK_TRACKID,
                        principalTable: "T_TRACK",
                        principalColumn: "PK_TRACKID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "T_ROLE",
                keyColumn: "PK_ROLEID",
                keyValue: new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"),
                column: "DT_CREATEDAT",
                value: new DateTime(2025, 3, 3, 21, 38, 42, 631, DateTimeKind.Utc).AddTicks(5359));

            migrationBuilder.UpdateData(
                table: "T_ROLE",
                keyColumn: "PK_ROLEID",
                keyValue: new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9b"),
                column: "DT_CREATEDAT",
                value: new DateTime(2025, 3, 3, 21, 38, 42, 631, DateTimeKind.Utc).AddTicks(5362));

            migrationBuilder.InsertData(
                table: "T_USER",
                columns: new[] { "PK_USERID", "DT_CREATEDAT", "TX_EMAIL", "TX_PASSWORD", "FL_DELETED", "FK_ROLEID", "DT_UPDATEDAT", "TX_USERNAME" },
                values: new object[,]
                {
                    { new Guid("0f9e5907-67f4-4d5d-93a2-d05bf6e817c0"), new DateTime(2025, 3, 3, 21, 38, 42, 749, DateTimeKind.Utc).AddTicks(2244), "lucascruzestudo@gmail.com", "$2a$11$IPEt2FHQwYPdpXVYhlwifuxQrT9GMjgjWrWhUwVUDYbImGFxYvYB.", false, new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"), null, "lucascruzestudo" },
                    { new Guid("1ebb6855-71db-48e0-9bfb-056cf944ef5d"), new DateTime(2025, 3, 3, 21, 38, 42, 631, DateTimeKind.Utc).AddTicks(5541), "admin@system.com", "$2a$11$/vcMOOStstzVnRSm1cZB7.qTjHG1yIwk9DUcpE1Ibth0rWmIbwp.O", false, new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"), null, "administrator" },
                    { new Guid("89881702-6a1d-46c3-9258-89e96b0376d3"), new DateTime(2025, 3, 3, 21, 38, 42, 868, DateTimeKind.Utc).AddTicks(1930), "lucascruztrabalho@gmail.com", "$2a$11$R8VTPL22rJXqXTV1XrllNOXZ5Nl2qFXsTVrZa8dnZqtu2U/WAVIoO", false, new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9b"), null, "lucascruztrabalho" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_TRACK_FK_USERID",
                table: "T_TRACK",
                column: "FK_USERID");

            migrationBuilder.CreateIndex(
                name: "IX_T_TRACK_TAG_FK_TAGID",
                table: "T_TRACK_TAG",
                column: "FK_TAGID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_TRACK_TAG");

            migrationBuilder.DropTable(
                name: "T_TAG");

            migrationBuilder.DropTable(
                name: "T_TRACK");

            migrationBuilder.DeleteData(
                table: "T_USER",
                keyColumn: "PK_USERID",
                keyValue: new Guid("0f9e5907-67f4-4d5d-93a2-d05bf6e817c0"));

            migrationBuilder.DeleteData(
                table: "T_USER",
                keyColumn: "PK_USERID",
                keyValue: new Guid("1ebb6855-71db-48e0-9bfb-056cf944ef5d"));

            migrationBuilder.DeleteData(
                table: "T_USER",
                keyColumn: "PK_USERID",
                keyValue: new Guid("89881702-6a1d-46c3-9258-89e96b0376d3"));

            migrationBuilder.UpdateData(
                table: "T_ROLE",
                keyColumn: "PK_ROLEID",
                keyValue: new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"),
                column: "DT_CREATEDAT",
                value: new DateTime(2025, 3, 2, 22, 12, 35, 350, DateTimeKind.Utc).AddTicks(4796));

            migrationBuilder.UpdateData(
                table: "T_ROLE",
                keyColumn: "PK_ROLEID",
                keyValue: new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9b"),
                column: "DT_CREATEDAT",
                value: new DateTime(2025, 3, 2, 22, 12, 35, 350, DateTimeKind.Utc).AddTicks(4800));

            migrationBuilder.InsertData(
                table: "T_USER",
                columns: new[] { "PK_USERID", "DT_CREATEDAT", "TX_EMAIL", "TX_PASSWORD", "FL_DELETED", "FK_ROLEID", "DT_UPDATEDAT", "TX_USERNAME" },
                values: new object[,]
                {
                    { new Guid("0bba6d29-5b5b-4cc2-8b6a-cfbe8f34e813"), new DateTime(2025, 3, 2, 22, 12, 35, 465, DateTimeKind.Utc).AddTicks(6856), "lucascruzestudo@gmail.com", "$2a$11$IIR3V.2fqeMZpjYMIC21L.ruj.NQJM3vxkBjehoW9IRDeXfRy1tOu", false, new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"), null, "lucascruzestudo" },
                    { new Guid("24ed49ca-b719-428f-8c58-83ec139cd326"), new DateTime(2025, 3, 2, 22, 12, 35, 582, DateTimeKind.Utc).AddTicks(7472), "lucascruztrabalho@gmail.com", "$2a$11$tUUMBJKzvw1OsU1RH6A60O4Mzyp1eBSXuejy1a45Bw320sIfIlGsu", false, new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9b"), null, "lucascruztrabalho" },
                    { new Guid("4154155f-f934-4cd8-b517-0a1114e3c11d"), new DateTime(2025, 3, 2, 22, 12, 35, 350, DateTimeKind.Utc).AddTicks(4984), "admin@system.com", "$2a$11$/GCuRGYpujcCJqx2NFMsu.caO/KR4WaC6HeTKtXu1YpPsKKWYtxF2", false, new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"), null, "administrator" }
                });
        }
    }
}
