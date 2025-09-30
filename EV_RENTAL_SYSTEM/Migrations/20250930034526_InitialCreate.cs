using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EV_RENTAL_SYSTEM.Migrations
{
    /// <summary>
    /// Initial Database Migration
    /// 
    /// CÁCH HOẠT ĐỘNG:
    /// - Tạo toàn bộ database schema
    /// - Tạo tất cả tables và relationships
    /// - Seed data cơ bản (Roles, LicenseTypes)
    /// 
    /// CHO TEAM:
    /// - KHÔNG cần xóa migration này
    /// - Chạy: dotnet ef database update
    /// - Data seeding sẽ tự động chạy sau migration
    /// 
    /// TABLES ĐƯỢC TẠO:
    /// - Role, User, License, LicenseType
    /// - Brand, Vehicle, Station, LicensePlate
    /// - Order, Contract, Payment, Transaction
    /// - Complaint, Maintenance, ProcessStep
    /// </summary>
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brand",
                columns: table => new
                {
                    Brand_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Brand_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brand", x => x.Brand_Id);
                });

            migrationBuilder.CreateTable(
                name: "LicenseType",
                columns: table => new
                {
                    License_type_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseType", x => x.License_type_Id);
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
                name: "Role",
                columns: table => new
                {
                    Role_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Role_Id);
                });

            migrationBuilder.CreateTable(
                name: "Station",
                columns: table => new
                {
                    Station_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Station_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Street = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    District = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Province = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Station", x => x.Station_Id);
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    Vehicle_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Model_year = table.Column<int>(type: "int", nullable: true),
                    Brand_Id = table.Column<int>(type: "int", nullable: false),
                    Vehicle_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Daily_rate = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Seat_number = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.Vehicle_Id);
                    table.ForeignKey(
                        name: "FK_Vehicle_Brand_Brand_Id",
                        column: x => x.Brand_Id,
                        principalTable: "Brand",
                        principalColumn: "Brand_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    User_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Birthday = table.Column<DateTime>(type: "date", nullable: true),
                    Created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Role_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.User_Id);
                    table.ForeignKey(
                        name: "FK_User_Role_Role_Id",
                        column: x => x.Role_Id,
                        principalTable: "Role",
                        principalColumn: "Role_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LicensePlate",
                columns: table => new
                {
                    License_plate_Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Vehicle_Id = table.Column<int>(type: "int", nullable: false),
                    Province = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Registration_date = table.Column<DateTime>(type: "date", nullable: true),
                    Condition = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Station_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicensePlate", x => x.License_plate_Id);
                    table.ForeignKey(
                        name: "FK_LicensePlate_Station_Station_Id",
                        column: x => x.Station_Id,
                        principalTable: "Station",
                        principalColumn: "Station_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LicensePlate_Vehicle_Vehicle_Id",
                        column: x => x.Vehicle_Id,
                        principalTable: "Vehicle",
                        principalColumn: "Vehicle_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "License",
                columns: table => new
                {
                    License_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    License_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Expiry_date = table.Column<DateTime>(type: "date", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    License_type_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    License_ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_License", x => x.License_Id);
                    table.ForeignKey(
                        name: "FK_License_LicenseType_License_type_Id",
                        column: x => x.License_type_Id,
                        principalTable: "LicenseType",
                        principalColumn: "License_type_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_License_User_User_Id",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "User_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Order_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Start_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    End_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Total_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    User_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Order_Id);
                    table.ForeignKey(
                        name: "FK_Order_User_User_Id",
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
                    License_plate_Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
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
                name: "Contract",
                columns: table => new
                {
                    Contract_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Order_Id = table.Column<int>(type: "int", nullable: false),
                    Created_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Deposit = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Rental_fee = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Extra_fee = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contract", x => x.Contract_Id);
                    table.ForeignKey(
                        name: "FK_Contract_Order_Order_Id",
                        column: x => x.Order_Id,
                        principalTable: "Order",
                        principalColumn: "Order_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Order_LicensePlate",
                columns: table => new
                {
                    Order_Id = table.Column<int>(type: "int", nullable: false),
                    License_plate_Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order_LicensePlate", x => new { x.Order_Id, x.License_plate_Id });
                    table.ForeignKey(
                        name: "FK_Order_LicensePlate_LicensePlate_License_plate_Id",
                        column: x => x.License_plate_Id,
                        principalTable: "LicensePlate",
                        principalColumn: "License_plate_Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Order_LicensePlate_Order_Order_Id",
                        column: x => x.Order_Id,
                        principalTable: "Order",
                        principalColumn: "Order_Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    Payment_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Payment_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Contract_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Payment_Id);
                    table.ForeignKey(
                        name: "FK_Payment_Contract_Contract_Id",
                        column: x => x.Contract_Id,
                        principalTable: "Contract",
                        principalColumn: "Contract_Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    Transaction_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Transaction_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Payment_Id = table.Column<int>(type: "int", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.Transaction_Id);
                    table.ForeignKey(
                        name: "FK_Transaction_Payment_Payment_Id",
                        column: x => x.Payment_Id,
                        principalTable: "Payment",
                        principalColumn: "Payment_Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transaction_User_User_Id",
                        column: x => x.User_Id,
                        principalTable: "User",
                        principalColumn: "User_Id",
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
                name: "IX_Complaint_Order_Id",
                table: "Complaint",
                column: "Order_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Complaint_User_Id",
                table: "Complaint",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Contract_Order_Id",
                table: "Contract",
                column: "Order_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ContractProcessing_Contract_Id",
                table: "ContractProcessing",
                column: "Contract_Id");

            migrationBuilder.CreateIndex(
                name: "IX_ContractProcessing_Step_Id",
                table: "ContractProcessing",
                column: "Step_Id");

            migrationBuilder.CreateIndex(
                name: "IX_License_License_number",
                table: "License",
                column: "License_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_License_License_type_Id",
                table: "License",
                column: "License_type_Id");

            migrationBuilder.CreateIndex(
                name: "IX_License_User_Id",
                table: "License",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_LicensePlate_Station_Id",
                table: "LicensePlate",
                column: "Station_Id");

            migrationBuilder.CreateIndex(
                name: "IX_LicensePlate_Vehicle_Id",
                table: "LicensePlate",
                column: "Vehicle_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Maintenance_License_plate_Id",
                table: "Maintenance",
                column: "License_plate_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_User_Id",
                table: "Order",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Order_LicensePlate_License_plate_Id",
                table: "Order_LicensePlate",
                column: "License_plate_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_Contract_Id",
                table: "Payment",
                column: "Contract_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_Payment_Id",
                table: "Transaction",
                column: "Payment_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_User_Id",
                table: "Transaction",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Role_Id",
                table: "User",
                column: "Role_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_Brand_Id",
                table: "Vehicle",
                column: "Brand_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Complaint");

            migrationBuilder.DropTable(
                name: "ContractProcessing");

            migrationBuilder.DropTable(
                name: "License");

            migrationBuilder.DropTable(
                name: "Maintenance");

            migrationBuilder.DropTable(
                name: "Order_LicensePlate");

            migrationBuilder.DropTable(
                name: "Transaction");

            migrationBuilder.DropTable(
                name: "ProcessStep");

            migrationBuilder.DropTable(
                name: "LicenseType");

            migrationBuilder.DropTable(
                name: "LicensePlate");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Station");

            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropTable(
                name: "Contract");

            migrationBuilder.DropTable(
                name: "Brand");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
