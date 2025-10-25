using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROYECTOMOVIE.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mi10namigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c040acd-d2fb-43ef-5fc5-c2e3f886ff01",
                column: "ConcurrencyStamp",
                value: "null");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3a8e1fdb-7c2d-4a5e-8f1c-9d3b2a1edf5c",
                column: "ConcurrencyStamp",
                value: "null");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c040acd-d2fb-43ef-5fc5-c2e3f886ff01",
                column: "ConcurrencyStamp",
                value: "97b51ccb-2b63-410f-9647-142f5bd7de9b");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3a8e1fdb-7c2d-4a5e-8f1c-9d3b2a1edf5c",
                column: "ConcurrencyStamp",
                value: "7a713f41-2f8d-4234-94a7-2f9de4fd5fe9");
        }
    }
}
