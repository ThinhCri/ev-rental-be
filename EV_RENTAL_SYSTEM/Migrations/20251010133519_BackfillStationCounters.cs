using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EV_RENTAL_SYSTEM.Migrations
{
    /// <inheritdoc />
    public partial class BackfillStationCounters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add columns only if they do not exist (guarded)
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

            // Backfill counters using LicensePlate only (robust even if Vehicle.Station_Id is removed)
            migrationBuilder.Sql(@"
-- Total vehicles per station = distinct Vehicle_Id among license plates at that station
UPDATE s
SET s.Total_Vehicle = totals.cnt
FROM [Station] s
OUTER APPLY (
    SELECT COUNT(DISTINCT lp.[Vehicle_Id]) AS cnt
    FROM [LicensePlate] lp
    WHERE lp.[Station_Id] = s.[Station_Id]
) totals;

-- Available vehicles per station = distinct Vehicle_Id with at least one license plate status 'Available' at that station
UPDATE s
SET s.Available_Vehicle = avail.cnt
FROM [Station] s
OUTER APPLY (
    SELECT COUNT(DISTINCT lp.[Vehicle_Id]) AS cnt
    FROM [LicensePlate] lp
    WHERE lp.[Station_Id] = s.[Station_Id]
      AND lp.[Status] = 'Available'
) avail;

-- Ensure no NULLs
UPDATE [Station] SET [Total_Vehicle] = 0 WHERE [Total_Vehicle] IS NULL;
UPDATE [Station] SET [Available_Vehicle] = 0 WHERE [Available_Vehicle] IS NULL;

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
