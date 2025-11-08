using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PROYECTOMOVIE.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mi8vamigracion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateTable(
                name: "t_Pelicula",
                columns: table => new
                {
                    Id_Peli = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre_Peli = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Imagen_Peli = table.Column<string>(type: "TEXT", nullable: false),
                    Descripción = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Enlace_Peli = table.Column<string>(type: "TEXT", nullable: false),
                    Tiempo_Duracion = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Video_Trailer = table.Column<string>(type: "TEXT", nullable: false),
                    Fecha_Publicada = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_Pelicula", x => x.Id_Peli);
                });

            migrationBuilder.CreateTable(
                name: "t_UsuarioPelicula",
                columns: table => new
                {
                    UsuarioId = table.Column<string>(type: "TEXT", nullable: false),
                    PeliculaId = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaAgregada = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EsFavorita = table.Column<bool>(type: "INTEGER", nullable: false),
                    Vista = table.Column<bool>(type: "INTEGER", nullable: false),
                    Calificacion = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_UsuarioPelicula", x => new { x.UsuarioId, x.PeliculaId });
                    table.ForeignKey(
                        name: "FK_t_UsuarioPelicula_t_Pelicula_PeliculaId",
                        column: x => x.PeliculaId,
                        principalTable: "t_Pelicula",
                        principalColumn: "Id_Peli",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_UsuarioPelicula_t_usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "t_usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_UsuarioPelicula_PeliculaId",
                table: "t_UsuarioPelicula",
                column: "PeliculaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_UsuarioPelicula");

            migrationBuilder.DropTable(
                name: "t_Pelicula");

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
                values: new object[] { "admin-id-123", 0, "--------", "CONCURRENCY_STAMP_ADMIN", "admin12345@movie.com", true, new DateTime(2025, 10, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), false, null, "Administrador Principal", "ADMIN@MOVIE.COM", "ADMIN@MOVIE.COM", "AQAAAAIAAYagAAAAEK63+OVx2qVlGfc8x9LSryS+GwhqeBI1A9ZxXaX+Y/VllEQtVB8UE2SXzM2mAbIWWQ==", null, true, "SECURITY_STAMP_ADMIN", false, "admin@movie.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "admin-id-123" });
        }
    }
}
