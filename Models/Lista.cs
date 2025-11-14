using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTOMOVIE.Models
{
    [Table("t_Lista")]
    public class Lista
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string? UsuarioId { get; set; }
        
        [Required(ErrorMessage = "El nombre de la lista es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string? Nombre { get; set; }
        
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string? Descripcion { get; set; }
        
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        public bool EsPublica { get; set; } = false;
        
        // Navegación
        public virtual Usuario? Usuario { get; set; }
        public virtual ICollection<ListaPelicula>? ListaPeliculas { get; set; }
    }
}