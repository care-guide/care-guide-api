using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareGuide.Infra.Migrations
{
    /// <inheritdoc />
    public partial class AddEventAddressesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "event_addresses",
                columns: table => new
                {
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    address_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_addresses", x => new { x.event_id, x.address_id });
                    table.ForeignKey(
                        name: "fk_event_address_address",
                        column: x => x.address_id,
                        principalTable: "addresses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_event_address_event",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_event_addresses_address_id",
                table: "event_addresses",
                column: "address_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "event_addresses");
        }
    }
}
