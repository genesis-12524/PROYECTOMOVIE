using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PROYECTOMOVIE.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", "1", "Administrador", "ADMINISTRADOR" },
                    { "2", "2", "Cliente", "CLIENTE" }
                });

            migrationBuilder.InsertData(
                table: "t_usuario",
                columns: new[] { "Id", "AccessFailedCount", "Apellido", "ConcurrencyStamp", "Email", "EmailConfirmed", "FechaRegistro", "LockoutEnabled", "LockoutEnd", "Nombre", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "admin-id-123", 0, "--------", "ea1b2b8f-fe2a-48e0-9024-e25d2ae79d9e", "admin12345@movie.com", true, new DateTime(2025, 10, 17, 16, 18, 37, 970, DateTimeKind.Local).AddTicks(1308), false, null, "Administrador Principal", "ADMIN@MOVIE.COM", "ADMIN@MOVIE.COM", "AQAAAAIAAYagAAAAECqiAZXuMe4yGqYBzqQaKaBMYhCrRV4a1IQX9zwqtgT6N8e+6/hhX+NTsEdl9Y/8mA==", null, true, "c384a30f-131b-4e1b-820c-caf6a8a86c07", false, "admin@movie.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "admin-id-123" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "admin-id-123" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "t_usuario",
                keyColumn: "Id",
                keyValue: "admin-id-123");
        }
    }
}
