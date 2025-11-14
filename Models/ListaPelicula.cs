using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTOMOVIE.Models
{
    [Table("t_ListaPelicula")]
    public class ListaPelicula
    {
        [Key]
        public int Id { get; set; }
        public int ListaId { get; set; }
        public int PeliculaId { get; set; }
        
        public DateTime FechaAgregada { get; set; } = DateTime.Now;
        public int Orden { get; set; }
        
        // Navegación
        public virtual Lista? Lista { get; set; }
        public virtual Pelicula? Pelicula { get; set; }
    }
}