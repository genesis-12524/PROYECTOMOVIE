using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PROYECTOMOVIE.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mi9namigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasPaymentMethod",
                table: "t_usuario",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PlanSubcripciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CicloFacturacion = table.Column<int>(type: "INTEGER", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanSubcripciones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioSuscripciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    PlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    MercadoPagoSubscriptionId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MercadoPagoPayerId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MercadoPagoCardId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NextBillingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NotificationSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastNotificationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UsuarioId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioSuscripciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuarioSuscripciones_PlanSubcripciones_PlanId",
                        column: x => x.PlanId,
                        principalTable: "PlanSubcripciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuarioSuscripciones_t_usuario_UserId",
                        column: x => x.UserId,
                        principalTable: "t_usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioSuscripciones_t_usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "t_usuario",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "2c040acd-d2fb-43ef-5fc5-c2e3f886ff01", "97b51ccb-2b63-410f-9647-142f5bd7de9b", "Cliente", "CLIENTE" },
                    { "3a8e1fdb-7c2d-4a5e-8f1c-9d3b2a1edf5c", "7a713f41-2f8d-4234-94a7-2f9de4fd5fe9", "Administrador", "ADMINISTRADOR" }
                });

            migrationBuilder.InsertData(
                table: "PlanSubcripciones",
                columns: new[] { "Id", "Activo", "CicloFacturacion", "Descripcion", "Nombre", "Precio" },
                values: new object[,]
                {
                    { 1, true, 30, "Ideal para un espectador - 1 pantalla, calidad HD", "Plan Básico", 14.90m },
                    { 2, true, 30, "Perfecto para familias - 3 pantallas, Full HD", "Plan Estándar", 24.90m },
                    { 3, true, 30, "Experiencia cinematográfica - 5 pantallas, calidad 4K", "Plan Premium", 34.90m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSuscripciones_PlanId",
                table: "UsuarioSuscripciones",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSuscripciones_Status_NextBillingDate",
                table: "UsuarioSuscripciones",
                columns: new[] { "Status", "NextBillingDate" });

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSuscripciones_UserId",
                table: "UsuarioSuscripciones",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioSuscripciones_UsuarioId",
                table: "UsuarioSuscripciones",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioSuscripciones");

            migrationBuilder.DropTable(
                name: "PlanSubcripciones");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2c040acd-d2fb-43ef-5fc5-c2e3f886ff01");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3a8e1fdb-7c2d-4a5e-8f1c-9d3b2a1edf5c");

            migrationBuilder.DropColumn(
                name: "HasPaymentMethod",
                table: "t_usuario");
        }
    }
}
