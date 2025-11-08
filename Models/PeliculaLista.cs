using System;

namespace PROYECTOMOVIE.Models
{
    public class PeliculaLista
    {
        public int Id { get; set; }
        public int ListaClienteId { get; set; }
        public int PeliculaId { get; set; }
        public DateTime FechaAgregado { get; set; }

        public virtual ListaCliente? ListaCliente { get; set; }
    }
}