using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PROYECTOMOVIE.Data;
using PROYECTOMOVIE.Models;
using Microsoft.Extensions.Logging;

namespace PROYECTOMOVIE.Areas.Identity.Pages.MercadoPago
{
    public class ResultadoModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ResultadoModel> _logger;

        public ResultadoModel(ApplicationDbContext context, ILogger<ResultadoModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string? collection_id { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? collection_status { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? external_reference { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? payment_type { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? merchant_order_id { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? preference_id { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? site_id { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? processing_mode { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? merchant_account_id { get; set; }

        public string PaymentId => collection_id ?? "N/A";
        public string Status => collection_status ?? "unknown";
        public string? Mensaje { get; set; }
        public decimal Amount { get; set; }
        public string FechaPago => DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        public string? NombrePlan { get; set; }
        public int? SubscriptionId { get; set; }
        
        public bool PagoExitoso => 
            !string.IsNullOrEmpty(collection_status) && 
            (collection_status.ToLower() == "approved" || collection_status.ToLower() == "completed");

        public async Task<IActionResult> OnGet()
        {
            _logger.LogInformation("=== PROCESANDO RESULTADO DE PAGO ===");
            _logger.LogInformation($"External Reference: {external_reference}");
            _logger.LogInformation($"Collection Status: {collection_status}");
            _logger.LogInformation($"Payment ID: {collection_id}");

            try
            {
                // ✅ RECUPERAR SUBSCRIPTION ID DEL EXTERNAL_REFERENCE
                if (int.TryParse(external_reference, out int subscriptionId))
                {
                    SubscriptionId = subscriptionId;
                    _logger.LogInformation($"✅ Subscription ID encontrado: {subscriptionId}");

                    // ✅ BUSCAR LA SUSCRIPCIÓN EN LA BASE DE DATOS
                    var subscription = await _context.Subscripciones
                        .Include(s => s.Plan)
                        .FirstOrDefaultAsync(s => s.Id == subscriptionId);

                    if (subscription != null)
                    {
                        _logger.LogInformation($"✅ Suscripción encontrada - Plan: {subscription.Plan?.Nombre}, Estado anterior: {subscription.Status}");

                        // ✅ ACTUALIZAR ESTADO DE LA SUSCRIPCIÓN SEGÚN EL PAGO
                        if (PagoExitoso)
                        {
                            subscription.Status = "active";
                            subscription.FechaInicio = DateTime.UtcNow;
                            subscription.SubscriptionIdMercadoPago = collection_id;
                            
                            _logger.LogInformation($"✅ Suscripción ACTIVADA - ID: {subscription.Id}");
                        }
                        else
                        {
                            subscription.Status = collection_status?.ToLower() ?? "failed";
                            _logger.LogInformation($"❌ Suscripción FALLIDA - Estado: {subscription.Status}");
                        }

                        // ✅ GUARDAR CAMBIOS EN LA BASE DE DATOS
                        await _context.SaveChangesAsync();
                        _logger.LogInformation($"✅ Base de datos actualizada");

                        // ✅ OBTENER INFORMACIÓN DEL PLAN PARA MOSTRAR
                        NombrePlan = subscription.Plan?.Nombre;
                        Amount = subscription.Plan?.Precio ?? 0;
                    }
                    else
                    {
                        _logger.LogWarning($"⚠️ Suscripción no encontrada para ID: {subscriptionId}");
                        Amount = 2999.99m; // Valor por defecto
                    }
                }
                else
                {
                    _logger.LogWarning($"⚠️ External Reference no válido: {external_reference}");
                    Amount = 2999.99m; // Valor por defecto
                }

                // ✅ DETERMINAR MENSAJE SEGÚN EL ESTADO
                Mensaje = collection_status?.ToLower() switch
                {
                    "approved" => "¡Felicidades! Tu pago ha sido aprobado exitosamente y tu suscripción está ahora activa.",
                    "pending" => "Tu pago está pendiente de confirmación. Te notificaremos cuando sea procesado.",
                    "rejected" => "Lo sentimos, tu pago ha sido rechazado. Por favor, intenta con otro método de pago.",
                    "cancelled" => "El pago ha sido cancelado. Puedes intentar nuevamente cuando lo desees.",
                    "in_process" => "El pago está en proceso de verificación. Esto puede tomar algunos minutos.",
                    "refunded" => "El pago ha sido reembolsado.",
                    "charged_back" => "Se ha realizado un contracargo en esta transacción.",
                    _ => "Estado del pago desconocido. Por favor, contacta con soporte."
                };

                _logger.LogInformation($"📋 Resultado final - Estado: {Status}, Mensaje: {Mensaje}, PagoExitoso: {PagoExitoso}");

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ ERROR procesando resultado de pago");
                
                // ✅ MANEJO DE ERRORES - Valores por defecto
                Mensaje = "Ocurrió un error al procesar el resultado del pago. Por favor, contacta con soporte.";
                Amount = 2999.99m;
                
                return Page();
            }
        }
    }
}