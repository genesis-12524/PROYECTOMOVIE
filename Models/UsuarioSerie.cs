using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTOMOVIE.Models
{
    // Define el nombre de la tabla en la base de datos
    [Table("t_UsuarioSerie")]
    public class UsuarioSerie
    {
        // Llave Foránea y Parte de la Llave Compuesta: ID del Usuario
        public string? UsuarioId { get; set; }

        // Llave Foránea y Parte de la Llave Compuesta: ID de la Serie
        public int? SerieId { get; set; } // Cambiado de PeliculaId a SerieId

        // --- Propiedades adicionales para la relación ---
        
        // Fecha en que el usuario agregó la serie a su lista
        public DateTime FechaAgregada { get; set; } = DateTime.Now;
        
        // Indicador si la serie es favorita del usuario
        public bool EsFavorita { get; set; } // si o no
        
        // Indicador si el usuario marcó la serie como vista (toda la serie)
        public bool Vista { get; set; } // si ya la vio o no
        
        // Calificación dada por el usuario (Ejemplo: 1 a 5 estrellas)
        public int? Calificacion { get; set; } // Ejemplo: 1 a 5 estrellas

        // --- Propiedades de navegación ---
        
        // Navegación hacia el modelo Usuario
        public virtual Usuario? Usuario { get; set; }
        
        // Navegación hacia el modelo Serie (Necesitarás esta clase creada)
        public virtual Serie? Serie { get; set; } // Cambiado de Pelicula a Serie
    }
}