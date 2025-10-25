using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTOMOVIE.Models
{
    public class PlanSubcripcion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // basic, standard, premium

        [Required]
        public string Nombre { get; set; } // basic, standard, premium

        [Required]
        public decimal Precio { get; set; }

        [StringLength(300)]
        public string Descripcion { get; set; }
        public int CicloFacturacion { get; set; } = 30;
        public bool Activo { get; set; } = true;
        
        // Relaciones
        public ICollection<UsuarioSuscripcion> UsuarioSuscripcion { get; set; }
    }
}