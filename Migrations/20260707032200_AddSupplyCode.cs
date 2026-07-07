using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStationery.Mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplyCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupplyCode",
                table: "Stationeries",
                type: "TEXT",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Stationeries",
                keyColumn: "Id",
                keyValue: 1,
                column: "SupplyCode",
                value: "VPP-0001");

            migrationBuilder.UpdateData(
                table: "Stationeries",
                keyColumn: "Id",
                keyValue: 2,
                column: "SupplyCode",
                value: "VPP-0002");

            migrationBuilder.UpdateData(
                table: "Stationeries",
                keyColumn: "Id",
                keyValue: 3,
                column: "SupplyCode",
                value: "VPP-0003");

            migrationBuilder.UpdateData(
                table: "Stationeries",
                keyColumn: "Id",
                keyValue: 4,
                column: "SupplyCode",
                value: "VPP-0004");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplyCode",
                table: "Stationeries");
        }
    }
}
