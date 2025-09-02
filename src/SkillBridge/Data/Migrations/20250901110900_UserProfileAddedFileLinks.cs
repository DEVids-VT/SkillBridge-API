using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserProfileAddedFileLinks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CVUpload",
                table: "user_profiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GitHubConnection",
                table: "user_profiles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "user_profiles",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CVUpload",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "GitHubConnection",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "user_profiles");
        }
    }
}
