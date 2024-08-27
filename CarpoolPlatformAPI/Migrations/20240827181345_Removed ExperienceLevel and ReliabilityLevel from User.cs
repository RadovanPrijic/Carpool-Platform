using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpoolPlatformAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemovedExperienceLevelandReliabilityLevelfromUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExperienceLevel",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ReliabilityLevel",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExperienceLevel",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReliabilityLevel",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
