using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareGuide.Infra.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsActiveHardDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "users");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "phones");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "person_healths");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "person_family_histories");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "person_diseases");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "person_annotations");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "files");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "events");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "event_surgeries");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "event_medicines");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "event_exams");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "event_appointments");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "doctors");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "doctor_specialties");

            migrationBuilder.DropColumn(
                name: "is_active",
                table: "addresses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "refresh_tokens",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "phones",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "persons",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "person_healths",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "person_family_histories",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "person_diseases",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "person_annotations",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "files",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "events",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "event_surgeries",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "event_medicines",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "event_exams",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "event_appointments",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "doctors",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "doctor_specialties",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "addresses",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }
    }
}
