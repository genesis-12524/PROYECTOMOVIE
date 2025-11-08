using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace PROYECTOMOVIE.Models
{
    public class PeliculaCreateModel
    {


        [Required]
        public string? Nombre_Peli { get; set; }

        [Required]
        public IFormFile? ImagenFile { get; set; } // Para subir la imagen

        [Required]
        public string? Descripción { get; set; }

        [Required]
        public IFormFile? PeliculaFile { get; set; } // Para subir el video de la película completa

        [Required]
        public TimeSpan Tiempo_Duracion { get; set; }

        [Required]
        public IFormFile? TrailerFile { get; set; } // Para subir el trailer

        [Required]
        public DateTime Fecha_Publicada { get; set; } = DateTime.Now;
    }
}
    
