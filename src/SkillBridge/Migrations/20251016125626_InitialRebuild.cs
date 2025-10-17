using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkillBridge.Migrations
{
    /// <inheritdoc />
    public partial class InitialRebuild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    about = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    logo_url = table.Column<string>(type: "text", nullable: true),
                    banner_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    activities = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    sector = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    head_office_location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    technologies = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    year_established = table.Column<int>(type: "integer", nullable: false),
                    has_offices_in_bulgaria = table.Column<bool>(type: "boolean", nullable: false),
                    bulgarian_office_locations = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    employees_in_bulgaria = table.Column<int>(type: "integer", nullable: true),
                    employees_worldwide = table.Column<int>(type: "integer", nullable: false),
                    why_work_with_us = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    website_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    contact_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    contact_email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    contact_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    auth0_user_id = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "skills",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_skills", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    profile_picture = table.Column<string>(type: "text", nullable: true),
                    cv_upload = table.Column<string>(type: "text", nullable: true),
                    github_connection = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profiles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "project_assignments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    summary = table.Column<string>(type: "text", nullable: false),
                    learning_benefits = table.Column<string>(type: "text", nullable: false),
                    suggested_approach = table.Column<string>(type: "text", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_assignments", x => x.id);
                    table.ForeignKey(
                        name: "FK_project_assignments_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "assignment_tasks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    project_assignment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assignment_tasks", x => x.id);
                    table.ForeignKey(
                        name: "FK_assignment_tasks_project_assignments_project_assignment_id",
                        column: x => x.project_assignment_id,
                        principalTable: "project_assignments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "project_skills",
                columns: table => new
                {
                    project_assignment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    skill_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_project_skills", x => new { x.project_assignment_id, x.skill_id });
                    table.ForeignKey(
                        name: "FK_project_skills_project_assignments_project_assignment_id",
                        column: x => x.project_assignment_id,
                        principalTable: "project_assignments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_project_skills_skills_skill_id",
                        column: x => x.skill_id,
                        principalTable: "skills",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_project_assignments",
                columns: table => new
                {
                    user_profile_id = table.Column<string>(type: "text", nullable: false),
                    project_assignment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claimed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    completed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deadline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    total_tasks = table.Column<int>(type: "integer", nullable: false),
                    completed_tasks = table.Column<int>(type: "integer", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "user_assignment_tasks",
                columns: table => new
                {
                    assignment_task_id = table.Column<Guid>(type: "uuid", nullable: false),
                    project_assignment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_profile_id = table.Column<string>(type: "text", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_assignment_tasks", x => new { x.user_profile_id, x.project_assignment_id, x.assignment_task_id });
                    table.ForeignKey(
                        name: "FK_user_assignment_tasks_assignment_tasks_assignment_task_id",
                        column: x => x.assignment_task_id,
                        principalTable: "assignment_tasks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_assignment_tasks_user_project_assignments_user_profile~",
                        columns: x => new { x.user_profile_id, x.project_assignment_id },
                        principalTable: "user_project_assignments",
                        principalColumns: new[] { "user_profile_id", "project_assignment_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_assignment_tasks_project_assignment_id",
                table: "assignment_tasks",
                column: "project_assignment_id");

            migrationBuilder.CreateIndex(
                name: "IX_assignment_tasks_sequence",
                table: "assignment_tasks",
                column: "sequence");

            migrationBuilder.CreateIndex(
                name: "IX_companies_auth0_user_id",
                table: "companies",
                column: "auth0_user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_companies_name",
                table: "companies",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_companies_sector",
                table: "companies",
                column: "sector");

            migrationBuilder.CreateIndex(
                name: "IX_project_assignments_company_id",
                table: "project_assignments",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_project_assignments_level",
                table: "project_assignments",
                column: "level");

            migrationBuilder.CreateIndex(
                name: "IX_project_assignments_status",
                table: "project_assignments",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "IX_project_skills_skill_id",
                table: "project_skills",
                column: "skill_id");

            migrationBuilder.CreateIndex(
                name: "IX_skills_name",
                table: "skills",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_assignment_tasks_assignment_task_id",
                table: "user_assignment_tasks",
                column: "assignment_task_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_assignment_tasks_user_project",
                table: "user_assignment_tasks",
                columns: new[] { "user_profile_id", "project_assignment_id" });

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
                name: "project_skills");

            migrationBuilder.DropTable(
                name: "user_assignment_tasks");

            migrationBuilder.DropTable(
                name: "skills");

            migrationBuilder.DropTable(
                name: "assignment_tasks");

            migrationBuilder.DropTable(
                name: "user_project_assignments");

            migrationBuilder.DropTable(
                name: "project_assignments");

            migrationBuilder.DropTable(
                name: "user_profiles");

            migrationBuilder.DropTable(
                name: "companies");
        }
    }
}
