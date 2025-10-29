using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EV_RENTAL_SYSTEM.Migrations
{
    /// <inheritdoc />
    public partial class EnsureStationVehicleCounters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Guarded SQL to add columns only if they do not exist
            migrationBuilder.Sql(@"
IF COL_LENGTH('Station', 'Available_Vehicle') IS NULL
BEGIN
    ALTER TABLE [Station] ADD [Available_Vehicle] int NOT NULL CONSTRAINT DF_Station_Available_Vehicle DEFAULT(0);
END
IF COL_LENGTH('Station', 'Total_Vehicle') IS NULL
BEGIN
    ALTER TABLE [Station] ADD [Total_Vehicle] int NOT NULL CONSTRAINT DF_Station_Total_Vehicle DEFAULT(0);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Available_Vehicle",
                table: "Station");

            migrationBuilder.DropColumn(
                name: "Total_Vehicle",
                table: "Station");
        }
    }
}
