using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBridge.Migrations
{
    /// <inheritdoc />
    public partial class TakeOffTotalTaskAndCompletedTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "completed_tasks",
                table: "user_project_assignments");

            migrationBuilder.DropColumn(
                name: "total_tasks",
                table: "user_project_assignments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "completed_tasks",
                table: "user_project_assignments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_tasks",
                table: "user_project_assignments",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
