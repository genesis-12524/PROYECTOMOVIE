namespace PROYECTOMOVIE.Models;
public class Subscripcion
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int PlanId { get; set; }
    public string SubscriptionIdMercadoPago { get; set; }
    public string Status { get; set; } // active, pending, cancelled, etc.
    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public virtual Usuario User { get; set; }
    public virtual Plan Plan { get; set; }
}