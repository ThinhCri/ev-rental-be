using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EV_RENTAL_SYSTEM.Migrations
{
    /// <inheritdoc />
    public partial class AddNotesToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Is_Booking_For_Others",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Renter_License_ImageUrl",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Renter_Name",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Renter_Phone",
                table: "Order");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Order",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Order");

            migrationBuilder.AddColumn<bool>(
                name: "Is_Booking_For_Others",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Renter_License_ImageUrl",
                table: "Order",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Renter_Name",
                table: "Order",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Renter_Phone",
                table: "Order",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }
    }
}
