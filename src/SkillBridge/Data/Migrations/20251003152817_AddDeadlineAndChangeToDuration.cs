using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDeadlineAndChangeToDuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_project_assignments_deadline",
                table: "project_assignments");

            migrationBuilder.DropColumn(
                name: "deadline",
                table: "project_assignments");

            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "user_project_assignments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "duration",
                table: "project_assignments",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AlterColumn<string>(
                name: "logo_url",
                table: "companies",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_project_assignments_duration",
                table: "project_assignments",
                column: "duration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_project_assignments_duration",
                table: "project_assignments");

            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "user_project_assignments");

            migrationBuilder.DropColumn(
                name: "duration",
                table: "project_assignments");

            migrationBuilder.AddColumn<DateTime>(
                name: "deadline",
                table: "project_assignments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "logo_url",
                table: "companies",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_project_assignments_deadline",
                table: "project_assignments",
                column: "deadline");
        }
    }
}
