using System;
using System.ComponentModel.DataAnnotations;

namespace PROYECTOMOVIE.Models
{
    public class Pelicula
    {
        public int Id { get; set; }

        [Required]
        public string? Nombre_Peli { get; set; }

        [Required]
        [Url]
        public string? Imagen_Peli { get; set; }

        public string? ImagenPublicId { get; set; }

        [Required]
        public string? Descripción { get; set; }

        [Required]
        [Url]
        public string? Enlace_Peli { get; set; }

        public string? PeliculaPublicId { get; set; }

        [Required]
        public TimeSpan Tiempo_Duracion { get; set; }

        [Required]
        [Url]
        public string? Video_Trailer { get; set; }

        public string? TrailerPublicId { get; set; }

        [Required]
        public DateTime Fecha_Publicada { get; set; } = DateTime.Now;
        
        //Relación muchos a muchos
        public virtual ICollection<UsuarioPelicula>? UsuarioPeliculas { get; set; }
        // Relación muchos a muchos con Categorias
        public virtual ICollection<Categoria>? Categorias { get; set; }
    }


}

