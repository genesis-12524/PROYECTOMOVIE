using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROYECTOMOVIE.Data.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSistemaListas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "t_Lista",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UsuarioId = table.Column<string>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EsPublica = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_Lista", x => x.Id);
                    table.ForeignKey(
                        name: "FK_t_Lista_t_usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "t_usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "t_ListaPelicula",
                columns: table => new
                {
                    ListaId = table.Column<int>(type: "INTEGER", nullable: false),
                    PeliculaId = table.Column<int>(type: "INTEGER", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaAgregada = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Orden = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_ListaPelicula", x => new { x.ListaId, x.PeliculaId });
                    table.ForeignKey(
                        name: "FK_t_ListaPelicula_Peliculas_PeliculaId",
                        column: x => x.PeliculaId,
                        principalTable: "Peliculas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_t_ListaPelicula_t_Lista_ListaId",
                        column: x => x.ListaId,
                        principalTable: "t_Lista",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_Lista_UsuarioId",
                table: "t_Lista",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_t_ListaPelicula_PeliculaId",
                table: "t_ListaPelicula",
                column: "PeliculaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_ListaPelicula");

            migrationBuilder.DropTable(
                name: "t_Lista");
        }
    }
}
