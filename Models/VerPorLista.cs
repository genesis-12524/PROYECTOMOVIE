
using System.Collections.Generic;

namespace PROYECTOMOVIE.Models // O el namespace que estés usando
{
    public class VerPorLista
    {
        // Esta propiedad contendrá la lista de películas
        public List<Pelicula> Peliculas { get; set; }
        
        // Esta propiedad contendrá la lista de series
        public List<Serie> Series { get; set; } // Asumo que tienes un modelo 'Serie'
        public List<Categoria> Categorias { get; set; } // <-- ¡NUEVA LÍNEA!
        public string? TerminoBusqueda { get; set; }
        public string? Imagen_Peli { get; set; }

        // Constructor para inicializar las listas y evitar errores
        public VerPorLista()
        {
            Peliculas = new List<Pelicula>();
            Series = new List<Serie>();
            Categorias = new List<Categoria>(); // <-- ¡NUEVA LÍNEA!
        }
    }
}