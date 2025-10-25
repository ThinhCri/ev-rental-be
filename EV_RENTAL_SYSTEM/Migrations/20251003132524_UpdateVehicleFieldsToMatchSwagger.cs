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

            migrationBuilder.AddColumn<int>(
                name: "Station_Id",
                table: "Vehicle",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_Station_Id",
                table: "Vehicle",
                column: "Station_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicle_Station_Station_Id",
                table: "Vehicle",
                column: "Station_Id",
                principalTable: "Station",
                principalColumn: "Station_Id");

            // Add Notes and StationId to User table
            migrationBuilder.AddColumn<string>(
                name: "Phone_number",
                table: "User",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "User",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Station_Id",
                table: "User",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Station_Id",
                table: "User",
                column: "Station_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Station_Station_Id",
                table: "User",
                column: "Station_Id",
                principalTable: "Station",
                principalColumn: "Station_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop User table changes
            migrationBuilder.DropForeignKey(
                name: "FK_User_Station_Station_Id",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_Station_Id",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Phone_number",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Station_Id",
                table: "User");

            // Drop Vehicle table changes
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_Station_Station_Id",
                table: "Vehicle");

            migrationBuilder.DropIndex(
                name: "IX_Vehicle_Station_Id",
                table: "Vehicle");

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
                name: "Station_Id",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Vehicle_image",
                table: "Vehicle");

            migrationBuilder.RenameColumn(
                name: "Price_per_day",
                table: "Vehicle",
                newName: "Daily_rate");

            // Add back Vehicle_type and Charging_time
            migrationBuilder.AddColumn<decimal>(
                name: "Charging_time",
                table: "Vehicle",
                type: "decimal(4,1)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Vehicle_type",
                table: "Vehicle",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
