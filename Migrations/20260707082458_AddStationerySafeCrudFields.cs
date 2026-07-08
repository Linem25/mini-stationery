using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniStationery.Mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddStationerySafeCrudFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Stationeries",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Stationeries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Stationeries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Stationeries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Stationeries",
                type: "BLOB",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Stationeries",
                type: "TEXT",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Stationeries",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "DeletedAt", "Description", "IsDeleted", "UpdatedAt" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, false, null });

            migrationBuilder.UpdateData(
                table: "Stationeries",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "DeletedAt", "Description", "IsDeleted", "UpdatedAt" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, false, null });

            migrationBuilder.UpdateData(
                table: "Stationeries",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "DeletedAt", "Description", "IsDeleted", "UpdatedAt" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, false, null });

            migrationBuilder.UpdateData(
                table: "Stationeries",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "DeletedAt", "Description", "IsDeleted", "UpdatedAt" },
                values: new object[] { new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, false, null });

            migrationBuilder.CreateIndex(
                name: "IX_Stationeries_SupplyCode",
                table: "Stationeries",
                column: "SupplyCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stationeries_SupplyCode",
                table: "Stationeries");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Stationeries");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Stationeries");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Stationeries");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Stationeries");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Stationeries");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Stationeries");
        }
    }
}
