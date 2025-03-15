using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneratingIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_T_TRACK_TAG_FK_TAGID",
                table: "T_TRACK_TAG",
                newName: "IX_TRACK_TAGS_TAGID");

            migrationBuilder.RenameIndex(
                name: "IX_T_TRACK_PLAY_FK_USERID",
                table: "T_TRACK_PLAY",
                newName: "IX_TRACKPLAY_USERID");

            migrationBuilder.RenameIndex(
                name: "IX_T_TRACK_PLAY_FK_TRACKID",
                table: "T_TRACK_PLAY",
                newName: "IX_TRACKPLAY_TRACKID");

            migrationBuilder.RenameIndex(
                name: "IX_T_TRACK_LIKE_FK_TRACKID",
                table: "T_TRACK_LIKE",
                newName: "IX_TRACKLIKE_TRACKID");

            migrationBuilder.CreateIndex(
                name: "IX_TRACK_TAGS_TRACKID",
                table: "T_TRACK_TAG",
                column: "FK_TRACKID");

            migrationBuilder.CreateIndex(
                name: "IX_TRACK_TAGS_TRACKID_TAGID",
                table: "T_TRACK_TAG",
                columns: new[] { "FK_TRACKID", "FK_TAGID" });

            migrationBuilder.CreateIndex(
                name: "IX_TRACKPLAY_CREATEDAT",
                table: "T_TRACK_PLAY",
                column: "DT_CREATEDAT");

            migrationBuilder.CreateIndex(
                name: "IX_TRACKPLAY_USERID_TRACKID",
                table: "T_TRACK_PLAY",
                columns: new[] { "FK_USERID", "FK_TRACKID" });

            migrationBuilder.CreateIndex(
                name: "IX_TrackLike_UserId",
                table: "T_TRACK_LIKE",
                column: "FK_USERID");

            migrationBuilder.CreateIndex(
                name: "IX_TRACKLIKE_USERID_TRACKID",
                table: "T_TRACK_LIKE",
                columns: new[] { "FK_USERID", "FK_TRACKID" });

            migrationBuilder.CreateIndex(
                name: "IX_TRACK_ISPUBLIC_CREATEDAT",
                table: "T_TRACK",
                columns: new[] { "FL_ISPUBLIC", "DT_CREATEDAT" });

            migrationBuilder.CreateIndex(
                name: "IX_TAG_CREATEDAT",
                table: "T_TAG",
                column: "DT_CREATEDAT");

            migrationBuilder.CreateIndex(
                name: "IX_TAG_ISDELETED",
                table: "T_TAG",
                column: "FL_DELETED");

            migrationBuilder.CreateIndex(
                name: "IX_TAG_NAME",
                table: "T_TAG",
                column: "TX_NAME");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TRACK_TAGS_TRACKID",
                table: "T_TRACK_TAG");

            migrationBuilder.DropIndex(
                name: "IX_TRACK_TAGS_TRACKID_TAGID",
                table: "T_TRACK_TAG");

            migrationBuilder.DropIndex(
                name: "IX_TRACKPLAY_CREATEDAT",
                table: "T_TRACK_PLAY");

            migrationBuilder.DropIndex(
                name: "IX_TRACKPLAY_USERID_TRACKID",
                table: "T_TRACK_PLAY");

            migrationBuilder.DropIndex(
                name: "IX_TrackLike_UserId",
                table: "T_TRACK_LIKE");

            migrationBuilder.DropIndex(
                name: "IX_TRACKLIKE_USERID_TRACKID",
                table: "T_TRACK_LIKE");

            migrationBuilder.DropIndex(
                name: "IX_TRACK_ISPUBLIC_CREATEDAT",
                table: "T_TRACK");

            migrationBuilder.DropIndex(
                name: "IX_TAG_CREATEDAT",
                table: "T_TAG");

            migrationBuilder.DropIndex(
                name: "IX_TAG_ISDELETED",
                table: "T_TAG");

            migrationBuilder.DropIndex(
                name: "IX_TAG_NAME",
                table: "T_TAG");

            migrationBuilder.RenameIndex(
                name: "IX_TRACK_TAGS_TAGID",
                table: "T_TRACK_TAG",
                newName: "IX_T_TRACK_TAG_FK_TAGID");

            migrationBuilder.RenameIndex(
                name: "IX_TRACKPLAY_USERID",
                table: "T_TRACK_PLAY",
                newName: "IX_T_TRACK_PLAY_FK_USERID");

            migrationBuilder.RenameIndex(
                name: "IX_TRACKPLAY_TRACKID",
                table: "T_TRACK_PLAY",
                newName: "IX_T_TRACK_PLAY_FK_TRACKID");

            migrationBuilder.RenameIndex(
                name: "IX_TRACKLIKE_TRACKID",
                table: "T_TRACK_LIKE",
                newName: "IX_T_TRACK_LIKE_FK_TRACKID");
        }
    }
}
