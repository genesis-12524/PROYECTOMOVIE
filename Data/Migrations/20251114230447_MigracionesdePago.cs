using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PROYECTOMOVIE.Data.Migrations
{
    /// <inheritdoc />
    public partial class MigracionesdePago : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioSuscripciones");

            migrationBuilder.DropTable(
                name: "PlanSubcripciones");

            migrationBuilder.DropColumn(
                name: "HasPaymentMethod",
                table: "t_usuario");

            migrationBuilder.CreateTable(
                name: "Planes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: false),
                    Precio = table.Column<decimal>(type: "TEXT", nullable: false),
                    MercadoPagoPlanId = table.Column<string>(type: "TEXT", nullable: false),
                    DuracionDias = table.Column<int>(type: "INTEGER", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscripciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    PlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    SubscriptionIdMercadoPago = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscripciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscripciones_Planes_PlanId",
                        column: x => x.PlanId,
                        principalTable: "Planes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscripciones_t_usuario_UserId",
                        column: x => x.UserId,
                        principalTable: "t_usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Planes",
                columns: new[] { "Id", "Activo", "Descripcion", "DuracionDias", "MercadoPagoPlanId", "Nombre", "Precio" },
                values: new object[,]
                {
                    { 1, true, "1 pantalla - Calidad HD", 30, "b8c80fe330ab4dcb8ab6bf29596c8e05", "Básico", 19.90m },
                    { 2, true, "4 pantallas - Calidad 4K", 30, "6f044d2acc954b0798c8440c384945df", "Premium", 29.90m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscripciones_PlanId",
                table: "Subscripciones",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscripciones_UserId",
                table: "Subscripciones",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subscripciones");

            migrationBuilder.DropTable(
                name: "Planes");

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
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    CicloFacturacion = table.Column<int>(type: "INTEGER", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
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
                    PlanId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastNotificationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MercadoPagoCardId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MercadoPagoPayerId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    MercadoPagoSubscriptionId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    NextBillingDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    NotificationSent = table.Column<bool>(type: "INTEGER", nullable: false),
                    StartDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
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
    }
}
