using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Project.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackAudioFileNameAndImageFileName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "TX_AUDIOFILENAME",
                table: "T_TRACK",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TX_IMAGEFILENAME",
                table: "T_TRACK",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "T_ROLE",
                keyColumn: "PK_ROLEID",
                keyValue: new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9a"),
                column: "DT_CREATEDAT",
                value: new DateTime(2025, 3, 4, 13, 34, 12, 272, DateTimeKind.Utc).AddTicks(5497));

            migrationBuilder.UpdateData(
                table: "T_ROLE",
                keyColumn: "PK_ROLEID",
                keyValue: new Guid("f7d4d7a9-4d1e-4a8d-9a8e-9b9a9b9a9b9b"),
                column: "DT_CREATEDAT",
                value: new DateTime(2025, 3, 4, 13, 34, 12, 272, DateTimeKind.Utc).AddTicks(5502));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "TX_AUDIOFILENAME",
                table: "T_TRACK");

            migrationBuilder.DropColumn(
                name: "TX_IMAGEFILENAME",
                table: "T_TRACK");

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
        }
    }
}
