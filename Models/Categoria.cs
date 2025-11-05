using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace PROYECTOMOVIE.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)] // Límite de caracteres para el nombre
        public string? Nombre { get; set; }

        // Propiedades de navegación para la relación Many-to-Many
        public virtual ICollection<Pelicula>? Peliculas { get; set; }
        public virtual ICollection<Serie>? Series { get; set; }
    }
}