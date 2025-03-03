using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Project.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "T_USER",
                keyColumn: "PK_USERID",
                keyValue: new Guid("1832cdd0-ed35-4a6c-bc7b-3d6d104b6c36"));

            migrationBuilder.DeleteData(
                table: "T_USER",
                keyColumn: "PK_USERID",
                keyValue: new Guid("61a864df-b35c-470d-8e8d-e1b025f3b1d1"));

            migrationBuilder.DeleteData(
                table: "T_USER",
                keyColumn: "PK_USERID",
                keyValue: new Guid("c1fc3f80-866a-44a4-a33f-21445909de1e"));

            migrationBuilder.CreateTable(
                name: "T_USERPROFILE",
                columns: table => new
                {
                    PK_USERPROFILEID = table.Column<Guid>(type: "uuid", nullable: false),
                    FK_USERID = table.Column<Guid>(type: "uuid", nullable: false),
                    TX_DISPLAYNAME = table.Column<string>(type: "text", nullable: true),
                    TX_BIO = table.Column<string>(type: "text", nullable: true),
                    TX_LOCATION = table.Column<string>(type: "text", nullable: true),
                    TX_PROFILEPICTUREURL = table.Column<string>(type: "text", nullable: true),
                    DT_CREATEDAT = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DT_UPDATEDAT = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FL_DELETED = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_USERPROFILE", x => x.PK_USERPROFILEID);
                    table.ForeignKey(
                        name: "FK_USERPROFILE_USER",
                        column: x => x.FK_USERID,
                        principalTable: "T_USER",
                        principalColumn: "PK_USERID",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_T_USERPROFILE_FK_USERID",
                table: "T_USERPROFILE",
                column: "FK_USERID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_USERPROFILE");

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

            migrationBuilder.UpdateData(
                table: "T_ROLE",
                keyColumn: "PK_ROLEID",
                keyValue: new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"),
                column: "DT_CREATEDAT",
                value: new DateTime(2024, 10, 10, 2, 50, 45, 245, DateTimeKind.Utc).AddTicks(6451));

            migrationBuilder.UpdateData(
                table: "T_ROLE",
                keyColumn: "PK_ROLEID",
                keyValue: new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9b"),
                column: "DT_CREATEDAT",
                value: new DateTime(2024, 10, 10, 2, 50, 45, 245, DateTimeKind.Utc).AddTicks(6454));

            migrationBuilder.InsertData(
                table: "T_USER",
                columns: new[] { "PK_USERID", "DT_CREATEDAT", "TX_EMAIL", "TX_PASSWORD", "FL_DELETED", "FK_ROLEID", "DT_UPDATEDAT", "TX_USERNAME" },
                values: new object[,]
                {
                    { new Guid("1832cdd0-ed35-4a6c-bc7b-3d6d104b6c36"), new DateTime(2024, 10, 10, 2, 50, 45, 360, DateTimeKind.Utc).AddTicks(1116), "lucascruzestudo@gmail.com", "$2a$11$moRhq6j4UrriJM/MqBdBmuCoep5.exC79fOYxzHkDGXllMA0ux1Wi", false, new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"), null, "lucascruzestudo" },
                    { new Guid("61a864df-b35c-470d-8e8d-e1b025f3b1d1"), new DateTime(2024, 10, 10, 2, 50, 45, 475, DateTimeKind.Utc).AddTicks(3673), "lucascruztrabalho@gmail.com", "$2a$11$KxH9z7sXaK4SsoA0N3L5N.b9RU/x/F1JplZhFNLg7fhWwb.MdFgf2", false, new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9b"), null, "lucascruztrabalho" },
                    { new Guid("c1fc3f80-866a-44a4-a33f-21445909de1e"), new DateTime(2024, 10, 10, 2, 50, 45, 245, DateTimeKind.Utc).AddTicks(6611), "admin@system.com", "$2a$11$udfkrLZ89y5yF055EkYgReShrlaASxyCvo4uQUDwm87Uzz.XSiaWC", false, new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"), null, "administrator" }
                });
        }
    }
}
