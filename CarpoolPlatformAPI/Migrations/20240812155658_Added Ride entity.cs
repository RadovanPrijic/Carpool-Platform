using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpoolPlatformAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedRideentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rides",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PricePerSeat = table.Column<double>(type: "float", nullable: false),
                    RideDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CarInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SeatsAvailable = table.Column<int>(type: "int", nullable: false),
                    TwoInBackseat = table.Column<bool>(type: "bit", nullable: false),
                    LuggageSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InsuranceStatus = table.Column<bool>(type: "bit", nullable: false),
                    AutomaticBooking = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rides", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rides");
        }
    }
}
