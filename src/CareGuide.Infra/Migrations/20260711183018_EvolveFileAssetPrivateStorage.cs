using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareGuide.Infra.Migrations
{
    /// <inheritdoc />
    public partial class EvolveFileAssetPrivateStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "content_type",
                table: "files",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "person_id",
                table: "files",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "storage_key",
                table: "files",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "storage_provider",
                table: "files",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "upload_status",
                table: "files",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_files_person_id",
                table: "files",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "IX_files_storage_key",
                table: "files",
                column: "storage_key",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_file_person",
                table: "files",
                column: "person_id",
                principalTable: "persons",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_file_person",
                table: "files");

            migrationBuilder.DropIndex(
                name: "IX_files_person_id",
                table: "files");

            migrationBuilder.DropIndex(
                name: "IX_files_storage_key",
                table: "files");

            migrationBuilder.DropColumn(
                name: "content_type",
                table: "files");

            migrationBuilder.DropColumn(
                name: "person_id",
                table: "files");

            migrationBuilder.DropColumn(
                name: "storage_key",
                table: "files");

            migrationBuilder.DropColumn(
                name: "storage_provider",
                table: "files");

            migrationBuilder.DropColumn(
                name: "upload_status",
                table: "files");
        }
    }
}
