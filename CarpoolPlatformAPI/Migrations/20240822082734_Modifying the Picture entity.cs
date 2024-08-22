using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpoolPlatformAPI.Migrations
{
    /// <inheritdoc />
    public partial class ModifyingthePictureentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileLocation",
                table: "Pictures",
                newName: "FilePath");

            migrationBuilder.AddColumn<string>(
                name: "FileExtension",
                table: "Pictures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "Pictures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSizeInBytes",
                table: "Pictures",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileExtension",
                table: "Pictures");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "Pictures");

            migrationBuilder.DropColumn(
                name: "FileSizeInBytes",
                table: "Pictures");

            migrationBuilder.RenameColumn(
                name: "FilePath",
                table: "Pictures",
                newName: "FileLocation");
        }
    }
}
