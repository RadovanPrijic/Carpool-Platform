using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpoolPlatformAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedStartLocationandEndLocationattributesinRide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EndLocation",
                table: "Rides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartLocation",
                table: "Rides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndLocation",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "StartLocation",
                table: "Rides");
        }
    }
}
