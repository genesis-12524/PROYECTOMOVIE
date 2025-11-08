using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROYECTOMOVIE.Data.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarRelacionGenerosAMuchosAMuchos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Genero",
                table: "Peliculas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Genero",
                table: "Peliculas",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
