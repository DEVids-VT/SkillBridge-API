using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubmissionUrlToUserProjectAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubmissionNotes",
                table: "user_project_assignments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubmissionRepositoryUrl",
                table: "user_project_assignments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "user_project_assignments",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmissionNotes",
                table: "user_project_assignments");

            migrationBuilder.DropColumn(
                name: "SubmissionRepositoryUrl",
                table: "user_project_assignments");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "user_project_assignments");
        }
    }
}
