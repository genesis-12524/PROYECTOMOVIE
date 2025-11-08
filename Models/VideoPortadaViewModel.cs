namespace PROYECTOMOVIE.Models
{
    public class VideoPortadaViewModel
    {
        public int? PeliculaSeleccionadaId { get; set; }
        public string? NombrePelicula { get; set; }
        public string? DescripcionPelicula { get; set; }
        public string? TrailerUrl { get; set; }
        public bool TieneVideoConfigurado { get; set; }
        public List<Pelicula> PeliculasDisponibles { get; set; } = new List<Pelicula>();
    
        // Información adicional de la configuración
        public DateTime? FechaConfiguracion { get; set; }
    }
}