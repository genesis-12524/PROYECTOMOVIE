using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PROYECTOMOVIE.Models
{
    public class UsuarioSuscripcion
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public int PlanId { get; set; }
        
        
        [ForeignKey("UserId")]
        public virtual Usuario Usuario { get; set; }
        
        [ForeignKey("PlanId")]
        public virtual PlanSubcripcion Plan { get; set; }
        
        
        
        // Datos de Mercado Pago
        public string? MercadoPagoSubscriptionId { get; set; }
        public string? MercadoPagoPayerId { get; set; }
        public string? MercadoPagoCardId { get; set; }

        // Estado y fechas
        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Pendiente;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime NextBillingDate { get; set; } = DateTime.UtcNow.AddDays(30);
        public DateTime? EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Notificaciones
        public bool NotificationSent { get; set; } = false;
        public DateTime? LastNotificationDate { get; set; }
    }

    public enum SubscriptionStatus
    {
        Activo,
        Pendiente,
        Cancelado,
        Expirado,
        PagoFallido
    }
}