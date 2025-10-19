using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTOMOVIE.Models
{
    [Table("t_UsuarioPelicula")]
    public class UsuarioPelicula
    {
            public string? UsuarioId { get; set; }
            public int? PeliculaId { get; set; }
    
            // Propiedades adicionales para la relación
            public DateTime FechaAgregada { get; set; } = DateTime.Now;
            public bool EsFavorita { get; set; }//si o no
            public bool Vista { get; set; } //si ya la vio o no
            public int? Calificacion { get; set; } // Ejemplo: 1 a 5 estrellas

            // Propiedades de navegación
            public virtual Usuario? Usuario { get; set; }
            public virtual Pelicula? Pelicula { get; set; }
    }
}