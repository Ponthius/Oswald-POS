using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oswald_POS.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkerToSales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkerId",
                table: "Sales",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_WorkerId",
                table: "Sales",
                column: "WorkerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Workers_WorkerId",
                table: "Sales",
                column: "WorkerId",
                principalTable: "Workers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Workers_WorkerId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_WorkerId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "WorkerId",
                table: "Sales");
        }
    }
}
