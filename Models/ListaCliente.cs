using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTOMOVIE.Models
{
    public class ListaCliente
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la lista es obligatorio.")]
        [StringLength(100)]
        public string? NombreLista { get; set; }

        [StringLength(300)]
        public string? Descripcion { get; set; }

        // Relación con el usuario (propietario de la lista)
        [Required]
        public string? UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }


        // Relación con los elementos (pelis, series, etc.)
        public virtual ICollection<ListaContenido> Items { get; set; } = new List<ListaContenido>();
    }
}
