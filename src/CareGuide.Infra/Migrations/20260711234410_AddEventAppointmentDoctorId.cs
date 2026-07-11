using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareGuide.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddEventAppointmentDoctorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "doctor_id",
                table: "event_appointments",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_appointments_doctor_id",
                table: "event_appointments",
                column: "doctor_id");

            migrationBuilder.AddForeignKey(
                name: "fk_event_appointment_doctor",
                table: "event_appointments",
                column: "doctor_id",
                principalTable: "doctors",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_event_appointment_doctor",
                table: "event_appointments");

            migrationBuilder.DropIndex(
                name: "IX_event_appointments_doctor_id",
                table: "event_appointments");

            migrationBuilder.DropColumn(
                name: "doctor_id",
                table: "event_appointments");
        }
    }
}
