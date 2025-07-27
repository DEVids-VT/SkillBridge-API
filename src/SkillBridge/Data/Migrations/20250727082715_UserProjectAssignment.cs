using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserProjectAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_project_assignments",
                columns: table => new
                {
                    user_profile_id = table.Column<string>(type: "text", nullable: false),
                    project_assignment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claimed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_project_assignments", x => new { x.user_profile_id, x.project_assignment_id });
                    table.ForeignKey(
                        name: "FK_user_project_assignments_project_assignments_project_assign~",
                        column: x => x.project_assignment_id,
                        principalTable: "project_assignments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_project_assignments_user_profiles_user_profile_id",
                        column: x => x.user_profile_id,
                        principalTable: "user_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_project_assignments_project_assignment_id",
                table: "user_project_assignments",
                column: "project_assignment_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_project_assignments_user_profile_id",
                table: "user_project_assignments",
                column: "user_profile_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_project_assignments");
        }
    }
}
