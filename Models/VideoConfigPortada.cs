using System;
using System.ComponentModel.DataAnnotations;
namespace PROYECTOMOVIE.Models
{
    public class VideoConfigPortada
    {
        [Key]
        public int Id { get; set; }
    
        [Required]
        public int PeliculaId { get; set; }
    
        public DateTime FechaConfiguracion { get; set; } = DateTime.Now;
        public bool EstaActivo { get; set; } = true;
    
        // Navigation property
        public virtual Pelicula Pelicula { get; set; }
    }
}
