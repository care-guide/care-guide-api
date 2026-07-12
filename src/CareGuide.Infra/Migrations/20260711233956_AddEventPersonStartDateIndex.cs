using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareGuide.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddEventPersonStartDateIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_events_person_id",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_start_date",
                table: "events");

            migrationBuilder.CreateIndex(
                name: "ix_events_person_id_start_date",
                table: "events",
                columns: new[] { "person_id", "start_date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_events_person_id_start_date",
                table: "events");

            migrationBuilder.CreateIndex(
                name: "IX_events_person_id",
                table: "events",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_events_start_date",
                table: "events",
                column: "start_date");
        }
    }
}
