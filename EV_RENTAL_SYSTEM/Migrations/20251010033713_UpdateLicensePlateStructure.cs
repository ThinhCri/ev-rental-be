using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EV_RENTAL_SYSTEM.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLicensePlateStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "License_plate_Id",
                table: "Order_LicensePlate",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "License_plate_Id",
                table: "Maintenance",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "License_plate_Id",
                table: "LicensePlate",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Plate_Number",
                table: "LicensePlate",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            // Remove Status column from Vehicle table
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Vehicle");

            // Remove Province column from LicensePlate table
            migrationBuilder.DropColumn(
                name: "Province",
                table: "LicensePlate");

            // Add Status column to Order table (guarded against duplicate)
            migrationBuilder.Sql(@"IF COL_LENGTH('Order', 'Status') IS NULL
BEGIN
    ALTER TABLE [Order] ADD [Status] nvarchar(50) NULL;
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Add back Province column to LicensePlate table
            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "LicensePlate",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            // Add back Status column to Vehicle table
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Vehicle",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            // Drop Status column from Order table
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Plate_Number",
                table: "LicensePlate");

            migrationBuilder.AlterColumn<string>(
                name: "License_plate_Id",
                table: "Order_LicensePlate",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "License_plate_Id",
                table: "Maintenance",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "License_plate_Id",
                table: "LicensePlate",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
