using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EV_RENTAL_SYSTEM.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
<<<<<<< HEAD
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Order",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
=======
            // Guard against duplicate column when database was created via EnsureCreated
            migrationBuilder.Sql(@"IF COL_LENGTH('Order', 'Status') IS NULL
BEGIN
    ALTER TABLE [Order] ADD [Status] nvarchar(50) NULL;
END");
>>>>>>> origin/Rental
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Order");
        }
    }
}






