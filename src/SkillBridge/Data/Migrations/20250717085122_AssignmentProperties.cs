using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class AssignmentProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "learning_benefits",
                table: "project_assignments",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "level",
                table: "project_assignments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "suggested_approach",
                table: "project_assignments",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "summary",
                table: "project_assignments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_project_assignments_level",
                table: "project_assignments",
                column: "level");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_project_assignments_level",
                table: "project_assignments");

            migrationBuilder.DropColumn(
                name: "learning_benefits",
                table: "project_assignments");

            migrationBuilder.DropColumn(
                name: "level",
                table: "project_assignments");

            migrationBuilder.DropColumn(
                name: "suggested_approach",
                table: "project_assignments");

            migrationBuilder.DropColumn(
                name: "summary",
                table: "project_assignments");
        }
    }
}
