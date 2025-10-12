using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EV_RENTAL_SYSTEM.Migrations
{
    /// <inheritdoc />
    public partial class AddContractCodeToContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Order_OrderId",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_License_LicenseType_LicenseTypeId",
                table: "License");

            migrationBuilder.DropForeignKey(
                name: "FK_License_User_UserId",
                table: "License");

            migrationBuilder.DropForeignKey(
                name: "FK_LicensePlate_Vehicle_VehicleId",
                table: "LicensePlate");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_User_UserId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_LicensePlate_LicensePlate_LicensePlateId",
                table: "Order_LicensePlate");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_LicensePlate_Order_OrderId",
                table: "Order_LicensePlate");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Contract_ContractId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Payment_PaymentId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_User_UserId",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_RoleId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Station_StationId",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_Brand_BrandId",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "VehicleCount",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "LicenseNumber",
                table: "LicensePlate");

            migrationBuilder.DropColumn(
                name: "LicenseImage",
                table: "License");

            migrationBuilder.RenameColumn(
                name: "VehicleImage",
                table: "Vehicle",
                newName: "Vehicle_image");

            migrationBuilder.RenameColumn(
                name: "SeatNumber",
                table: "Vehicle",
                newName: "Seat_number");

            migrationBuilder.RenameColumn(
                name: "PricePerDay",
                table: "Vehicle",
                newName: "Price_per_day");

            migrationBuilder.RenameColumn(
                name: "BrandId",
                table: "Vehicle",
                newName: "Brand_Id");

            migrationBuilder.RenameColumn(
                name: "VehicleId",
                table: "Vehicle",
                newName: "Vehicle_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicle_BrandId",
                table: "Vehicle",
                newName: "IX_Vehicle_Brand_Id");

            migrationBuilder.RenameColumn(
                name: "StationId",
                table: "User",
                newName: "Station_Id");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "User",
                newName: "Role_Id");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "User",
                newName: "Phone_number");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "User",
                newName: "Full_name");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "User",
                newName: "Created_at");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "User",
                newName: "User_Id");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "User",
                newName: "Password");

            migrationBuilder.RenameIndex(
                name: "IX_User_StationId",
                table: "User",
                newName: "IX_User_Station_Id");

            migrationBuilder.RenameIndex(
                name: "IX_User_RoleId",
                table: "User",
                newName: "IX_User_Role_Id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Transaction",
                newName: "User_Id");

            migrationBuilder.RenameColumn(
                name: "TransactionDate",
                table: "Transaction",
                newName: "Transaction_date");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "Transaction",
                newName: "Payment_Id");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "Transaction",
                newName: "Transaction_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_UserId",
                table: "Transaction",
                newName: "IX_Transaction_User_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_PaymentId",
                table: "Transaction",
                newName: "IX_Transaction_Payment_Id");

            migrationBuilder.RenameColumn(
                name: "StationName",
                table: "Station",
                newName: "Station_name");

            migrationBuilder.RenameColumn(
                name: "StationId",
                table: "Station",
                newName: "Station_Id");

            migrationBuilder.RenameColumn(
                name: "RoleName",
                table: "Role",
                newName: "Role_name");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "Role",
                newName: "Role_Id");

            migrationBuilder.RenameColumn(
                name: "PaymentDate",
                table: "Payment",
                newName: "Payment_date");

            migrationBuilder.RenameColumn(
                name: "ContractId",
                table: "Payment",
                newName: "Contract_Id");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "Payment",
                newName: "Payment_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_ContractId",
                table: "Payment",
                newName: "IX_Payment_Contract_Id");

            migrationBuilder.RenameColumn(
                name: "LicensePlateId",
                table: "Order_LicensePlate",
                newName: "License_plate_Id");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Order_LicensePlate",
                newName: "Order_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Order_LicensePlate_LicensePlateId",
                table: "Order_LicensePlate",
                newName: "IX_Order_LicensePlate_License_plate_Id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Order",
                newName: "User_Id");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Order",
                newName: "Total_amount");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Order",
                newName: "Start_time");

            migrationBuilder.RenameColumn(
                name: "OrderDate",
                table: "Order",
                newName: "Order_date");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "Order",
                newName: "End_time");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Order",
                newName: "Order_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Order_UserId",
                table: "Order",
                newName: "IX_Order_User_Id");

            migrationBuilder.RenameColumn(
                name: "TypeName",
                table: "LicenseType",
                newName: "Type_name");

            migrationBuilder.RenameColumn(
                name: "LicenseTypeId",
                table: "LicenseType",
                newName: "License_type_Id");

            migrationBuilder.RenameColumn(
                name: "VehicleId",
                table: "LicensePlate",
                newName: "Vehicle_Id");

            migrationBuilder.RenameColumn(
                name: "LicensePlateId",
                table: "LicensePlate",
                newName: "License_plate_Id");

            migrationBuilder.RenameIndex(
                name: "IX_LicensePlate_VehicleId",
                table: "LicensePlate",
                newName: "IX_LicensePlate_Vehicle_Id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "License",
                newName: "User_Id");

            migrationBuilder.RenameColumn(
                name: "LicenseTypeId",
                table: "License",
                newName: "License_type_Id");

            migrationBuilder.RenameColumn(
                name: "LicenseNumber",
                table: "License",
                newName: "License_number");

            migrationBuilder.RenameColumn(
                name: "ExpiryDate",
                table: "License",
                newName: "Expiry_date");

            migrationBuilder.RenameColumn(
                name: "LicenseId",
                table: "License",
                newName: "License_Id");

            migrationBuilder.RenameIndex(
                name: "IX_License_UserId",
                table: "License",
                newName: "IX_License_User_Id");

            migrationBuilder.RenameIndex(
                name: "IX_License_LicenseTypeId",
                table: "License",
                newName: "IX_License_License_type_Id");

            migrationBuilder.RenameColumn(
                name: "RentalFee",
                table: "Contract",
                newName: "Rental_fee");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Contract",
                newName: "Order_Id");

            migrationBuilder.RenameColumn(
                name: "ExtraFee",
                table: "Contract",
                newName: "Extra_fee");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Contract",
                newName: "Created_date");

            migrationBuilder.RenameColumn(
                name: "ContractId",
                table: "Contract",
                newName: "Contract_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Contract_OrderId",
                table: "Contract",
                newName: "IX_Contract_Order_Id");

            migrationBuilder.RenameColumn(
                name: "BrandName",
                table: "Brand",
                newName: "Brand_name");

            migrationBuilder.RenameColumn(
                name: "BrandId",
                table: "Brand",
                newName: "Brand_Id");

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "Vehicle",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price_per_day",
                table: "Vehicle",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Battery",
                table: "Vehicle",
                type: "decimal(5,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Vehicle",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Model_year",
                table: "Vehicle",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Range_km",
                table: "Vehicle",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Birthday",
                table: "User",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone_number",
                table: "User",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Full_name",
                table: "User",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created_at",
                table: "User",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Transaction",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Station_name",
                table: "Station",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "Available_Vehicle",
                table: "Station",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Station",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Station",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Province",
                table: "Station",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Station",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Total_Vehicle",
                table: "Station",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Role",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Payment",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Total_amount",
                table: "Order",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Start_time",
                table: "Order",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "End_time",
                table: "Order",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Type_name",
                table: "LicenseType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "License_type_Id",
                table: "LicenseType",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "LicenseType",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Plate_Number",
                table: "LicensePlate",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Registration_date",
                table: "LicensePlate",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Station_Id",
                table: "LicensePlate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "License_type_Id",
                table: "License",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Expiry_date",
                table: "License",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "License_ImageUrl",
                table: "License",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Deposit",
                table: "Contract",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Rental_fee",
                table: "Contract",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Extra_fee",
                table: "Contract",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Contract_Code",
                table: "Contract",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Complaint",
                columns: table => new
                {
                    Complaint_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Complaint_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Order_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Complaint", x => x.Complaint_Id);
                    table.ForeignKey(
                        name: "FK_Complaint_Order_Order_Id",
                        column: x => x.Order_Id,
                        principalTable: "Order",
                        principalColumn: "Order_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Complaint_User_User_Id",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Maintenance",
                columns: table => new
                {
                    Maintenance_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Maintenance_date = table.Column<DateTime>(type: "date", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    License_plate_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Maintenance", x => x.Maintenance_Id);
                    table.ForeignKey(
                        name: "FK_Maintenance_LicensePlate_License_plate_Id",
                        column: x => x.License_plate_Id,
                        principalTable: "LicensePlate",
                        principalColumn: "License_plate_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProcessStep",
                columns: table => new
                {
                    Step_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Step_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Terms = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessStep", x => x.Step_Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractProcessing",
                columns: table => new
                {
                    ContractProcessing_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Contract_Id = table.Column<int>(type: "int", nullable: false),
                    Step_Id = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractProcessing", x => x.ContractProcessing_Id);
                    table.ForeignKey(
                        name: "FK_ContractProcessing_Contract_Contract_Id",
                        column: x => x.Contract_Id,
                        principalTable: "Contract",
                        principalColumn: "Contract_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ContractProcessing_ProcessStep_Step_Id",
                        column: x => x.Step_Id,
                        principalTable: "ProcessStep",
                        principalColumn: "Step_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "LicenseType",
                columns: new[] { "License_type_Id", "Description", "Type_name" },
                values: new object[,]
                {
                    { "A", "Unlimited motorcycle", "A" },
                    { "A1", "Motorcycle up to 125cc", "A1" },
                    { "A2", "Motorcycle up to 175cc", "A2" },
                    { "B1", "Car up to 9 seats", "B1" },
                    { "B2", "Unlimited car", "B2" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Role_Id", "Description", "Role_name" },
                values: new object[,]
                {
                    { 1, "System Administrator", "Admin" },
                    { 2, "Station Staff Member", "Station Staff" },
                    { 3, "Electric Vehicle Renter", "EV Renter" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LicensePlate_Station_Id",
                table: "LicensePlate",
                column: "Station_Id");

            migrationBuilder.CreateIndex(
                name: "IX_License_License_number",
                table: "License",
                column: "License_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_Order_Id",
                table: "Complaint",
                column: "Order_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_User_Id",
                table: "Complaint",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ContractProcessing_Contract_Id",
                table: "ContractProcessing",
                column: "Contract_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ContractProcessing_Step_Id",
                table: "ContractProcessing",
                column: "Step_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenance_License_plate_Id",
                table: "Maintenance",
                column: "License_plate_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Order_Order_Id",
                table: "Contract",
                column: "Order_Id",
                principalTable: "Order",
                principalColumn: "Order_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_License_LicenseType_License_type_Id",
                table: "License",
                column: "License_type_Id",
                principalTable: "LicenseType",
                principalColumn: "License_type_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_License_User_User_Id",
                table: "License",
                column: "User_Id",
                principalTable: "User",
                principalColumn: "User_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LicensePlate_Station_Station_Id",
                table: "LicensePlate",
                column: "Station_Id",
                principalTable: "Station",
                principalColumn: "Station_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LicensePlate_Vehicle_Vehicle_Id",
                table: "LicensePlate",
                column: "Vehicle_Id",
                principalTable: "Vehicle",
                principalColumn: "Vehicle_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_User_User_Id",
                table: "Order",
                column: "User_Id",
                principalTable: "User",
                principalColumn: "User_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_LicensePlate_LicensePlate_License_plate_Id",
                table: "Order_LicensePlate",
                column: "License_plate_Id",
                principalTable: "LicensePlate",
                principalColumn: "License_plate_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_LicensePlate_Order_Order_Id",
                table: "Order_LicensePlate",
                column: "Order_Id",
                principalTable: "Order",
                principalColumn: "Order_Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Contract_Contract_Id",
                table: "Payment",
                column: "Contract_Id",
                principalTable: "Contract",
                principalColumn: "Contract_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Payment_Payment_Id",
                table: "Transaction",
                column: "Payment_Id",
                principalTable: "Payment",
                principalColumn: "Payment_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_User_User_Id",
                table: "Transaction",
                column: "User_Id",
                principalTable: "User",
                principalColumn: "User_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_Role_Id",
                table: "User",
                column: "Role_Id",
                principalTable: "Role",
                principalColumn: "Role_Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Station_Station_Id",
                table: "User",
                column: "Station_Id",
                principalTable: "Station",
                principalColumn: "Station_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicle_Brand_Brand_Id",
                table: "Vehicle",
                column: "Brand_Id",
                principalTable: "Brand",
                principalColumn: "Brand_Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contract_Order_Order_Id",
                table: "Contract");

            migrationBuilder.DropForeignKey(
                name: "FK_License_LicenseType_License_type_Id",
                table: "License");

            migrationBuilder.DropForeignKey(
                name: "FK_License_User_User_Id",
                table: "License");

            migrationBuilder.DropForeignKey(
                name: "FK_LicensePlate_Station_Station_Id",
                table: "LicensePlate");

            migrationBuilder.DropForeignKey(
                name: "FK_LicensePlate_Vehicle_Vehicle_Id",
                table: "LicensePlate");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_User_User_Id",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_LicensePlate_LicensePlate_License_plate_Id",
                table: "Order_LicensePlate");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_LicensePlate_Order_Order_Id",
                table: "Order_LicensePlate");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Contract_Contract_Id",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Payment_Payment_Id",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_User_User_Id",
                table: "Transaction");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Role_Role_Id",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_User_Station_Station_Id",
                table: "User");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_Brand_Brand_Id",
                table: "Vehicle");

            migrationBuilder.DropTable(
                name: "Complaint");

            migrationBuilder.DropTable(
                name: "ContractProcessing");

            migrationBuilder.DropTable(
                name: "Maintenance");

            migrationBuilder.DropTable(
                name: "ProcessStep");

            migrationBuilder.DropIndex(
                name: "IX_User_Email",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_LicensePlate_Station_Id",
                table: "LicensePlate");

            migrationBuilder.DropIndex(
                name: "IX_License_License_number",
                table: "License");

            migrationBuilder.DeleteData(
                table: "LicenseType",
                keyColumn: "License_type_Id",
                keyValue: "A");

            migrationBuilder.DeleteData(
                table: "LicenseType",
                keyColumn: "License_type_Id",
                keyValue: "A1");

            migrationBuilder.DeleteData(
                table: "LicenseType",
                keyColumn: "License_type_Id",
                keyValue: "A2");

            migrationBuilder.DeleteData(
                table: "LicenseType",
                keyColumn: "License_type_Id",
                keyValue: "B1");

            migrationBuilder.DeleteData(
                table: "LicenseType",
                keyColumn: "License_type_Id",
                keyValue: "B2");

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Role_Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Role_Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Role_Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "Battery",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Model_year",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Range_km",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Available_Vehicle",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "Province",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "Total_Vehicle",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "LicenseType");

            migrationBuilder.DropColumn(
                name: "Plate_Number",
                table: "LicensePlate");

            migrationBuilder.DropColumn(
                name: "Registration_date",
                table: "LicensePlate");

            migrationBuilder.DropColumn(
                name: "Station_Id",
                table: "LicensePlate");

            migrationBuilder.DropColumn(
                name: "License_ImageUrl",
                table: "License");

            migrationBuilder.DropColumn(
                name: "Contract_Code",
                table: "Contract");

            migrationBuilder.RenameColumn(
                name: "Vehicle_image",
                table: "Vehicle",
                newName: "VehicleImage");

            migrationBuilder.RenameColumn(
                name: "Seat_number",
                table: "Vehicle",
                newName: "SeatNumber");

            migrationBuilder.RenameColumn(
                name: "Price_per_day",
                table: "Vehicle",
                newName: "PricePerDay");

            migrationBuilder.RenameColumn(
                name: "Brand_Id",
                table: "Vehicle",
                newName: "BrandId");

            migrationBuilder.RenameColumn(
                name: "Vehicle_Id",
                table: "Vehicle",
                newName: "VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicle_Brand_Id",
                table: "Vehicle",
                newName: "IX_Vehicle_BrandId");

            migrationBuilder.RenameColumn(
                name: "Station_Id",
                table: "User",
                newName: "StationId");

            migrationBuilder.RenameColumn(
                name: "Role_Id",
                table: "User",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "Phone_number",
                table: "User",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Full_name",
                table: "User",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "Created_at",
                table: "User",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "User_Id",
                table: "User",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "User",
                newName: "PasswordHash");

            migrationBuilder.RenameIndex(
                name: "IX_User_Station_Id",
                table: "User",
                newName: "IX_User_StationId");

            migrationBuilder.RenameIndex(
                name: "IX_User_Role_Id",
                table: "User",
                newName: "IX_User_RoleId");

            migrationBuilder.RenameColumn(
                name: "User_Id",
                table: "Transaction",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Transaction_date",
                table: "Transaction",
                newName: "TransactionDate");

            migrationBuilder.RenameColumn(
                name: "Payment_Id",
                table: "Transaction",
                newName: "PaymentId");

            migrationBuilder.RenameColumn(
                name: "Transaction_Id",
                table: "Transaction",
                newName: "TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_User_Id",
                table: "Transaction",
                newName: "IX_Transaction_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Transaction_Payment_Id",
                table: "Transaction",
                newName: "IX_Transaction_PaymentId");

            migrationBuilder.RenameColumn(
                name: "Station_name",
                table: "Station",
                newName: "StationName");

            migrationBuilder.RenameColumn(
                name: "Station_Id",
                table: "Station",
                newName: "StationId");

            migrationBuilder.RenameColumn(
                name: "Role_name",
                table: "Role",
                newName: "RoleName");

            migrationBuilder.RenameColumn(
                name: "Role_Id",
                table: "Role",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "Payment_date",
                table: "Payment",
                newName: "PaymentDate");

            migrationBuilder.RenameColumn(
                name: "Contract_Id",
                table: "Payment",
                newName: "ContractId");

            migrationBuilder.RenameColumn(
                name: "Payment_Id",
                table: "Payment",
                newName: "PaymentId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_Contract_Id",
                table: "Payment",
                newName: "IX_Payment_ContractId");

            migrationBuilder.RenameColumn(
                name: "License_plate_Id",
                table: "Order_LicensePlate",
                newName: "LicensePlateId");

            migrationBuilder.RenameColumn(
                name: "Order_Id",
                table: "Order_LicensePlate",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_LicensePlate_License_plate_Id",
                table: "Order_LicensePlate",
                newName: "IX_Order_LicensePlate_LicensePlateId");

            migrationBuilder.RenameColumn(
                name: "User_Id",
                table: "Order",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Total_amount",
                table: "Order",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "Start_time",
                table: "Order",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "Order_date",
                table: "Order",
                newName: "OrderDate");

            migrationBuilder.RenameColumn(
                name: "End_time",
                table: "Order",
                newName: "EndTime");

            migrationBuilder.RenameColumn(
                name: "Order_Id",
                table: "Order",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_User_Id",
                table: "Order",
                newName: "IX_Order_UserId");

            migrationBuilder.RenameColumn(
                name: "Type_name",
                table: "LicenseType",
                newName: "TypeName");

            migrationBuilder.RenameColumn(
                name: "License_type_Id",
                table: "LicenseType",
                newName: "LicenseTypeId");

            migrationBuilder.RenameColumn(
                name: "Vehicle_Id",
                table: "LicensePlate",
                newName: "VehicleId");

            migrationBuilder.RenameColumn(
                name: "License_plate_Id",
                table: "LicensePlate",
                newName: "LicensePlateId");

            migrationBuilder.RenameIndex(
                name: "IX_LicensePlate_Vehicle_Id",
                table: "LicensePlate",
                newName: "IX_LicensePlate_VehicleId");

            migrationBuilder.RenameColumn(
                name: "User_Id",
                table: "License",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "License_type_Id",
                table: "License",
                newName: "LicenseTypeId");

            migrationBuilder.RenameColumn(
                name: "License_number",
                table: "License",
                newName: "LicenseNumber");

            migrationBuilder.RenameColumn(
                name: "Expiry_date",
                table: "License",
                newName: "ExpiryDate");

            migrationBuilder.RenameColumn(
                name: "License_Id",
                table: "License",
                newName: "LicenseId");

            migrationBuilder.RenameIndex(
                name: "IX_License_User_Id",
                table: "License",
                newName: "IX_License_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_License_License_type_Id",
                table: "License",
                newName: "IX_License_LicenseTypeId");

            migrationBuilder.RenameColumn(
                name: "Rental_fee",
                table: "Contract",
                newName: "RentalFee");

            migrationBuilder.RenameColumn(
                name: "Order_Id",
                table: "Contract",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "Extra_fee",
                table: "Contract",
                newName: "ExtraFee");

            migrationBuilder.RenameColumn(
                name: "Created_date",
                table: "Contract",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "Contract_Id",
                table: "Contract",
                newName: "ContractId");

            migrationBuilder.RenameIndex(
                name: "IX_Contract_Order_Id",
                table: "Contract",
                newName: "IX_Contract_OrderId");

            migrationBuilder.RenameColumn(
                name: "Brand_name",
                table: "Brand",
                newName: "BrandName");

            migrationBuilder.RenameColumn(
                name: "Brand_Id",
                table: "Brand",
                newName: "BrandId");

            migrationBuilder.AlterColumn<string>(
                name: "Model",
                table: "Vehicle",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerDay",
                table: "Vehicle",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Birthday",
                table: "User",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "User",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "User",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "User",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Transaction",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StationName",
                table: "Station",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Station",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleCount",
                table: "Station",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Payment",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Order",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartTime",
                table: "Order",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndTime",
                table: "Order",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TypeName",
                table: "LicenseType",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "LicenseTypeId",
                table: "LicenseType",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "LicenseNumber",
                table: "LicensePlate",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "LicenseTypeId",
                table: "License",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "License",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "LicenseImage",
                table: "License",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Deposit",
                table: "Contract",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "RentalFee",
                table: "Contract",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExtraFee",
                table: "Contract",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Contract_Order_OrderId",
                table: "Contract",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_License_LicenseType_LicenseTypeId",
                table: "License",
                column: "LicenseTypeId",
                principalTable: "LicenseType",
                principalColumn: "LicenseTypeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_License_User_UserId",
                table: "License",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LicensePlate_Vehicle_VehicleId",
                table: "LicensePlate",
                column: "VehicleId",
                principalTable: "Vehicle",
                principalColumn: "VehicleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_User_UserId",
                table: "Order",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_LicensePlate_LicensePlate_LicensePlateId",
                table: "Order_LicensePlate",
                column: "LicensePlateId",
                principalTable: "LicensePlate",
                principalColumn: "LicensePlateId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_LicensePlate_Order_OrderId",
                table: "Order_LicensePlate",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "OrderId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Contract_ContractId",
                table: "Payment",
                column: "ContractId",
                principalTable: "Contract",
                principalColumn: "ContractId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Payment_PaymentId",
                table: "Transaction",
                column: "PaymentId",
                principalTable: "Payment",
                principalColumn: "PaymentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_User_UserId",
                table: "Transaction",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Role_RoleId",
                table: "User",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "RoleId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Station_StationId",
                table: "User",
                column: "StationId",
                principalTable: "Station",
                principalColumn: "StationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicle_Brand_BrandId",
                table: "Vehicle",
                column: "BrandId",
                principalTable: "Brand",
                principalColumn: "BrandId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
