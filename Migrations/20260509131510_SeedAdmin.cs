using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oswald_POS.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Workers",
                columns: new[] { "Id", "FullName", "Password", "Role", "Username" },
                values: new object[] { 1, "System Admin", "admin123", "Admin", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Workers",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
