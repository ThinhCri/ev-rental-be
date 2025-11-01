using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EV_RENTAL_SYSTEM.Migrations
{
    /// <inheritdoc />
    public partial class AddRentalWorkflowFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Order");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Transaction",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Check_In_Status",
                table: "Order",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rental_Status",
                table: "Order",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Verification_License_ImageUrl",
                table: "Order",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VehicleConditionHistory",
                columns: table => new
                {
                    VehicleConditionHistory_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order_Id = table.Column<int>(type: "int", nullable: false),
                    License_Plate_Id = table.Column<int>(type: "int", nullable: true),
                    Status_Change_Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Vehicle_Images_Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Odometer_Reading = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Battery_Percentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Condition_Before = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Condition_After = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Change_Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Additional_Charges = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Additional_Charges_Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Staff_Notes = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleConditionHistory", x => x.VehicleConditionHistory_Id);
                    table.ForeignKey(
                        name: "FK_VehicleConditionHistory_LicensePlate_License_Plate_Id",
                        column: x => x.License_Plate_Id,
                        principalTable: "LicensePlate",
                        principalColumn: "License_plate_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleConditionHistory_Order_Order_Id",
                        column: x => x.Order_Id,
                        principalTable: "Order",
                        principalColumn: "Order_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleConditionHistory_License_Plate_Id",
                table: "VehicleConditionHistory",
                column: "License_Plate_Id");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleConditionHistory_Order_Id",
                table: "VehicleConditionHistory",
                column: "Order_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleConditionHistory");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "Check_In_Status",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Rental_Status",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Verification_License_ImageUrl",
                table: "Order");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Transaction",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Order",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }
    }
}
