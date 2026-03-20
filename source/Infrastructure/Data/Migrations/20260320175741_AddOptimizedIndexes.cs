using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddOptimizedIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_USERFOLLOW_FOLLOWER_FOLLOWED",
                table: "T_USER_FOLLOW");

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
                name: "IX_TRACKLIKE_USERID_TRACKID",
                table: "T_TRACK_LIKE");

            migrationBuilder.RenameIndex(
                name: "IX_T_USERPROFILE_FK_USERID",
                table: "T_USERPROFILE",
                newName: "IX_USERPROFILE_USERID");

            migrationBuilder.RenameIndex(
                name: "IX_TrackLike_UserId",
                table: "T_TRACK_LIKE",
                newName: "IX_TRACKLIKE_USERID");

            migrationBuilder.RenameIndex(
                name: "IX_T_TRACK_COMMENT_FK_USERID",
                table: "T_TRACK_COMMENT",
                newName: "IX_TRACKCOMMENT_USERID");

            migrationBuilder.RenameIndex(
                name: "IX_T_TRACK_COMMENT_FK_TRACKID",
                table: "T_TRACK_COMMENT",
                newName: "IX_TRACKCOMMENT_TRACKID");

            migrationBuilder.RenameIndex(
                name: "IX_T_TRACK_COMMENT_FK_PARENTCOMMENTID",
                table: "T_TRACK_COMMENT",
                newName: "IX_TRACKCOMMENT_PARENTCOMMENTID");

            migrationBuilder.RenameIndex(
                name: "IX_T_TRACK_FK_USERID",
                table: "T_TRACK",
                newName: "IX_TRACK_USERID");

            migrationBuilder.CreateIndex(
                name: "IX_USERPROFILE_DISPLAYNAME",
                table: "T_USERPROFILE",
                column: "TX_DISPLAYNAME");

            migrationBuilder.CreateIndex(
                name: "IX_USER_EMAIL",
                table: "T_USER",
                column: "TX_EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_USER_USERNAME",
                table: "T_USER",
                column: "TX_USERNAME",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TRACKPLAY_USERID_TRACKID_CREATEDAT",
                table: "T_TRACK_PLAY",
                columns: new[] { "FK_USERID", "FK_TRACKID", "DT_CREATEDAT" });

            migrationBuilder.CreateIndex(
                name: "IX_TRACKCOMMENT_TRACKID_CREATEDAT",
                table: "T_TRACK_COMMENT",
                columns: new[] { "FK_TRACKID", "DT_CREATEDAT" });

            migrationBuilder.CreateIndex(
                name: "IX_TRACK_USERID_ISPUBLIC_CREATEDAT",
                table: "T_TRACK",
                columns: new[] { "FK_USERID", "FL_ISPUBLIC", "DT_CREATEDAT" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_USERPROFILE_DISPLAYNAME",
                table: "T_USERPROFILE");

            migrationBuilder.DropIndex(
                name: "IX_USER_EMAIL",
                table: "T_USER");

            migrationBuilder.DropIndex(
                name: "IX_USER_USERNAME",
                table: "T_USER");

            migrationBuilder.DropIndex(
                name: "IX_TRACKPLAY_USERID_TRACKID_CREATEDAT",
                table: "T_TRACK_PLAY");

            migrationBuilder.DropIndex(
                name: "IX_TRACKCOMMENT_TRACKID_CREATEDAT",
                table: "T_TRACK_COMMENT");

            migrationBuilder.DropIndex(
                name: "IX_TRACK_USERID_ISPUBLIC_CREATEDAT",
                table: "T_TRACK");

            migrationBuilder.RenameIndex(
                name: "IX_USERPROFILE_USERID",
                table: "T_USERPROFILE",
                newName: "IX_T_USERPROFILE_FK_USERID");

            migrationBuilder.RenameIndex(
                name: "IX_TRACKLIKE_USERID",
                table: "T_TRACK_LIKE",
                newName: "IX_TrackLike_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TRACKCOMMENT_USERID",
                table: "T_TRACK_COMMENT",
                newName: "IX_T_TRACK_COMMENT_FK_USERID");

            migrationBuilder.RenameIndex(
                name: "IX_TRACKCOMMENT_TRACKID",
                table: "T_TRACK_COMMENT",
                newName: "IX_T_TRACK_COMMENT_FK_TRACKID");

            migrationBuilder.RenameIndex(
                name: "IX_TRACKCOMMENT_PARENTCOMMENTID",
                table: "T_TRACK_COMMENT",
                newName: "IX_T_TRACK_COMMENT_FK_PARENTCOMMENTID");

            migrationBuilder.RenameIndex(
                name: "IX_TRACK_USERID",
                table: "T_TRACK",
                newName: "IX_T_TRACK_FK_USERID");

            migrationBuilder.CreateIndex(
                name: "IX_USERFOLLOW_FOLLOWER_FOLLOWED",
                table: "T_USER_FOLLOW",
                columns: new[] { "FK_FOLLOWERUSERID", "FK_FOLLOWEDUSERID" });

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
                name: "IX_TRACKLIKE_USERID_TRACKID",
                table: "T_TRACK_LIKE",
                columns: new[] { "FK_USERID", "FK_TRACKID" });
        }
    }
}
