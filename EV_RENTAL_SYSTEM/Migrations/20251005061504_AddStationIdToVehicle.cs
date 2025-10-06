using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EV_RENTAL_SYSTEM.Migrations
{
    /// <inheritdoc />
    public partial class AddStationIdToVehicle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_Station_Station_Id",
                table: "Vehicle");

            migrationBuilder.DropIndex(
                name: "IX_Vehicle_Station_Id",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Station_Id",
                table: "Vehicle");
        }
    }
}
