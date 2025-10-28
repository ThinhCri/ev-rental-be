using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EV_RENTAL_SYSTEM.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVehicleFieldsToMatchSwagger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Daily_rate",
                table: "Vehicle",
                newName: "Price_per_day");

            migrationBuilder.AddColumn<decimal>(
                name: "Battery",
                table: "Vehicle",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Charging_time",
                table: "Vehicle",
                type: "decimal(4,1)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Range_km",
                table: "Vehicle",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Vehicle",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Vehicle_image",
                table: "Vehicle",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Battery",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Charging_time",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Range_km",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Vehicle_image",
                table: "Vehicle");

            migrationBuilder.RenameColumn(
                name: "Price_per_day",
                table: "Vehicle",
                newName: "Daily_rate");
        }
    }
}
