using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUserProfileConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfilePicture",
                table: "user_profiles",
                newName: "profile_picture");

            migrationBuilder.RenameColumn(
                name: "GitHubConnection",
                table: "user_profiles",
                newName: "github_connection");

            migrationBuilder.RenameColumn(
                name: "CVUpload",
                table: "user_profiles",
                newName: "cv_upload");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "profile_picture",
                table: "user_profiles",
                newName: "ProfilePicture");

            migrationBuilder.RenameColumn(
                name: "github_connection",
                table: "user_profiles",
                newName: "GitHubConnection");

            migrationBuilder.RenameColumn(
                name: "cv_upload",
                table: "user_profiles",
                newName: "CVUpload");
        }
    }
}
