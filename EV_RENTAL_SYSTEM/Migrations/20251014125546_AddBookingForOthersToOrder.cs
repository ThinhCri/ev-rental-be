using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EV_RENTAL_SYSTEM.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderAndContractStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop Status column from Contract table
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Contract");

            // Add Notes column to Order table (final result after both migrations)
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
            // Drop Notes column from Order table
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Order");

            // Add back Status column to Contract table
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Contract",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
