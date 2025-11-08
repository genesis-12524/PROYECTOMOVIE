using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROYECTOMOVIE.Data.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablasSeriesYRelaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Series",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre_Serie = table.Column<string>(type: "TEXT", nullable: false),
                    Imagen_Serie = table.Column<string>(type: "TEXT", nullable: false),
                    ImagenPublicId = table.Column<string>(type: "TEXT", nullable: true),
                    Descripción = table.Column<string>(type: "TEXT", nullable: false),
                    Enlace_Serie = table.Column<string>(type: "TEXT", nullable: false),
                    SeriePublicId = table.Column<string>(type: "TEXT", nullable: true),
                    Video_Trailer = table.Column<string>(type: "TEXT", nullable: false),
                    TrailerPublicId = table.Column<string>(type: "TEXT", nullable: true),
                    Fecha_Publicada = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Series", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "t_UsuarioSerie",
                columns: table => new
                {
                    UsuarioId = table.Column<string>(type: "TEXT", nullable: false),
                    SerieId = table.Column<int>(type: "INTEGER", nullable: false),
                    FechaAgregada = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EsFavorita = table.Column<bool>(type: "INTEGER", nullable: false),
                    Vista = table.Column<bool>(type: "INTEGER", nullable: false),
                    Calificacion = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_t_UsuarioSerie", x => new { x.UsuarioId, x.SerieId });
                    table.ForeignKey(
                        name: "FK_t_UsuarioSerie_Series_SerieId",
                        column: x => x.SerieId,
                        principalTable: "Series",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_t_UsuarioSerie_t_usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "t_usuario",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_t_UsuarioSerie_SerieId",
                table: "t_UsuarioSerie",
                column: "SerieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "t_UsuarioSerie");

            migrationBuilder.DropTable(
                name: "Series");
        }
    }
}
