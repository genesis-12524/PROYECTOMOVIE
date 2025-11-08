using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTOMOVIE.Models
{
    public class ListaContenido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ListaClienteId { get; set; }

        [ForeignKey("ListaClienteId")]
        public virtual ListaCliente? ListaCliente { get; set; }

        // IDs opcionales según tu tipo de contenido
        public int? PeliculaId { get; set; }
        public int? SerieId { get; set; }

        // Relaciones opcionales (si tienes modelos Pelicula o Serie)
        [ForeignKey("PeliculaId")]
        public virtual Pelicula? Pelicula { get; set; }

        [ForeignKey("SerieId")]
        public virtual Serie? Serie { get; set; }
    }
}

