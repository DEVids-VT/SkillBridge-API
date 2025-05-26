using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBridge.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatecompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "description",
                table: "companies",
                newName: "website_url");

            migrationBuilder.AddColumn<string>(
                name: "about",
                table: "companies",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "activities",
                table: "companies",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "banner_url",
                table: "companies",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "bulgarian_office_locations",
                table: "companies",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_info",
                table: "companies",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "employees_in_bulgaria",
                table: "companies",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "employees_worldwide",
                table: "companies",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "has_offices_in_bulgaria",
                table: "companies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "head_office_location",
                table: "companies",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "logo_url",
                table: "companies",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sector",
                table: "companies",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "technologies",
                table: "companies",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "why_work_with_us",
                table: "companies",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "year_established",
                table: "companies",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_companies_name",
                table: "companies",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_companies_sector",
                table: "companies",
                column: "sector");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_companies_name",
                table: "companies");

            migrationBuilder.DropIndex(
                name: "IX_companies_sector",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "about",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "activities",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "banner_url",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "bulgarian_office_locations",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "contact_info",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "employees_in_bulgaria",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "employees_worldwide",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "has_offices_in_bulgaria",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "head_office_location",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "logo_url",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "sector",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "technologies",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "why_work_with_us",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "year_established",
                table: "companies");

            migrationBuilder.RenameColumn(
                name: "website_url",
                table: "companies",
                newName: "description");
        }
    }
}
