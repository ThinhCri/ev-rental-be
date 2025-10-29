using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EV_RENTAL_SYSTEM.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingForOthersToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Contract");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "Status",
                table: "Contract",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
