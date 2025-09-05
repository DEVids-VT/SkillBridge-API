using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class DescriptionChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "project_assignments",
                type: "text",
                maxLength: 20000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20000)",
                oldMaxLength: 20000,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "project_assignments",
                type: "character varying(20000)",
                maxLength: 20000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldMaxLength: 20000,
                oldNullable: true);
        }
    }
}
