
namespace PROYECTOMOVIE.Models;
public class Plan
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public string MercadoPagoPlanId { get; set; } // ID del plan en Mercado Pago
    public int DuracionDias { get; set; }
    // Propiedades adicionales útiles
    public bool Activo { get; set; } = true;
}