using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EV_RENTAL_SYSTEM.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleImagesColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Chỉ thêm 2 cột mới, không drop gì cả
            migrationBuilder.AddColumn<string>(
                name: "Handover_Image",
                table: "Contract",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Return_Image",
                table: "Contract",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Chỉ xóa 2 cột đã thêm
            migrationBuilder.DropColumn(
                name: "Handover_Image",
                table: "Contract");

            migrationBuilder.DropColumn(
                name: "Return_Image",
                table: "Contract");
        }
    }
}
