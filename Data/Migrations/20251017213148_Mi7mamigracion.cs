using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROYECTOMOVIE.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mi7mamigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "t_usuario",
                keyColumn: "Id",
                keyValue: "admin-id-123",
                columns: new[] { "ConcurrencyStamp", "FechaRegistro", "PasswordHash", "SecurityStamp" },
                values: new object[] { "CONCURRENCY_STAMP_ADMIN", new DateTime(2025, 10, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "AQAAAAIAAYagAAAAEK63+OVx2qVlGfc8x9LSryS+GwhqeBI1A9ZxXaX+Y/VllEQtVB8UE2SXzM2mAbIWWQ==", "SECURITY_STAMP_ADMIN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "t_usuario",
                keyColumn: "Id",
                keyValue: "admin-id-123",
                columns: new[] { "ConcurrencyStamp", "FechaRegistro", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ea1b2b8f-fe2a-48e0-9024-e25d2ae79d9e", new DateTime(2025, 10, 17, 16, 18, 37, 970, DateTimeKind.Local).AddTicks(1308), "AQAAAAIAAYagAAAAECqiAZXuMe4yGqYBzqQaKaBMYhCrRV4a1IQX9zwqtgT6N8e+6/hhX+NTsEdl9Y/8mA==", "c384a30f-131b-4e1b-820c-caf6a8a86c07" });
        }
    }
}
