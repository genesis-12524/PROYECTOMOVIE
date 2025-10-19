using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROYECTOMOVIE.Data.Migrations
{
    /// <inheritdoc />
    public partial class Mi8vamn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_UsuarioPelicula_t_Pelicula_PeliculaId",
                table: "t_UsuarioPelicula");

            migrationBuilder.DropPrimaryKey(
                name: "PK_t_Pelicula",
                table: "t_Pelicula");

            migrationBuilder.RenameTable(
                name: "t_Pelicula",
                newName: "Peliculas");

            migrationBuilder.RenameColumn(
                name: "Id_Peli",
                table: "Peliculas",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "ImagenPublicId",
                table: "Peliculas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PeliculaPublicId",
                table: "Peliculas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrailerPublicId",
                table: "Peliculas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Peliculas",
                table: "Peliculas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_t_UsuarioPelicula_Peliculas_PeliculaId",
                table: "t_UsuarioPelicula",
                column: "PeliculaId",
                principalTable: "Peliculas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_t_UsuarioPelicula_Peliculas_PeliculaId",
                table: "t_UsuarioPelicula");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Peliculas",
                table: "Peliculas");

            migrationBuilder.DropColumn(
                name: "ImagenPublicId",
                table: "Peliculas");

            migrationBuilder.DropColumn(
                name: "PeliculaPublicId",
                table: "Peliculas");

            migrationBuilder.DropColumn(
                name: "TrailerPublicId",
                table: "Peliculas");

            migrationBuilder.RenameTable(
                name: "Peliculas",
                newName: "t_Pelicula");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "t_Pelicula",
                newName: "Id_Peli");

            migrationBuilder.AddPrimaryKey(
                name: "PK_t_Pelicula",
                table: "t_Pelicula",
                column: "Id_Peli");

            migrationBuilder.AddForeignKey(
                name: "FK_t_UsuarioPelicula_t_Pelicula_PeliculaId",
                table: "t_UsuarioPelicula",
                column: "PeliculaId",
                principalTable: "t_Pelicula",
                principalColumn: "Id_Peli",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
