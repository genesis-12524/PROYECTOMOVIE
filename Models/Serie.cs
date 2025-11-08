using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Asegúrate de incluir este para ICollection

namespace PROYECTOMOVIE.Models
{
    public class Serie
    {
        public int Id { get; set; }

        [Required]
        public string? Nombre_Serie { get; set; } // Cambiado de Peli a Serie

        [Required]
        [Url]
        public string? Imagen_Serie { get; set; } // Cambiado de Peli a Serie

        public string? ImagenPublicId { get; set; }

        [Required]
        public string? Descripción { get; set; }

        [Required]
        [Url]
        public string? Enlace_Serie { get; set; } // Cambiado de Peli a Serie

        public string? SeriePublicId { get; set; } // Cambiado de PeliculaPublicId

        // Nota: Para series, podrías querer un campo para el número de temporadas o episodios
        // Por simplicidad, mantendremos la estructura similar a la película por ahora.
        // Si necesitas una duración total, podrías usar TimeSpan.

        [Required]
        [Url]
        public string? Video_Trailer { get; set; }

        public string? TrailerPublicId { get; set; }

        [Required]
        public DateTime Fecha_Publicada { get; set; } = DateTime.Now;

        // Relación muchos a muchos (si aplica, similar a Pelicula)
        // Necesitarías crear una clase UsuarioSerie similar a UsuarioPelicula si la relación es la misma.
        public virtual ICollection<UsuarioSerie>? UsuarioSeries { get; set; }
        // Relación muchos a muchos con Categorias
        public virtual ICollection<Categoria>? Categorias { get; set; }
    }
}