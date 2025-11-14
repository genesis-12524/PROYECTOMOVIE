using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using PROYECTOMOVIE.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PROYECTOMOVIE.interfaze;
using Microsoft.AspNetCore.Http;
using PROYECTOMOVIE.Data;

namespace PROYECTOMOVIE.Services
{
    public class MercadoPagoService : IMercadoPagoService
    {
        private readonly IConfiguration _configuration;
        private readonly string _accessToken;
        private readonly bool _isSandbox;
        private readonly ILogger<MercadoPagoService> _logger;
        private readonly ApplicationDbContext _context;

        public MercadoPagoService(
            IConfiguration configuration, 
            ILogger<MercadoPagoService> logger,
            ApplicationDbContext context)
        {
            _configuration = configuration;
            _accessToken = _configuration["MercadoPago:AccessToken"];
            _isSandbox = _accessToken?.StartsWith("TEST-") ?? true;
            _logger = logger;
            _context = context;
        }

        // ✅ MÉTODO PRINCIPAL CORREGIDO: Crear suscripción recurrente
        public async Task<string> CreateSubscription(Plan plan, Usuario user, string returnUrl)
        {
            _logger.LogInformation("=== CREANDO SUSCRIPCIÓN RECURRENTE ===");
            _logger.LogInformation($"Plan: {plan.Nombre}, Usuario: {user.Email}");

            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                // ✅ PASO 1: Crear o obtener el Plan en MercadoPago
                var mercadoPagoPlanId = await GetOrCreateMercadoPagoPlan(plan);
                
                _logger.LogInformation($"Plan ID MercadoPago: {mercadoPagoPlanId}");

                // ✅ PASO 2: Crear la Suscripción
                var subscriptionRequest = new
                {
                    preapproval_plan_id = mercadoPagoPlanId,
                    reason = $"Suscripción {plan.Nombre} - PROYECTOMOVIE",
                    external_reference = $"user_{user.Id}_plan_{plan.Id}",
                    payer_email = user.Email,
                    auto_recurring = new
                    {
                        frequency = 1,
                        frequency_type = "months",
                        transaction_amount = (double)plan.Precio,
                        currency_id = "PEN",
                        start_date = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                    },
                    back_url = returnUrl,
                    status = "authorized"
                };

                var json = JsonSerializer.Serialize(subscriptionRequest, new JsonSerializerOptions 
                { 
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
                });

                _logger.LogInformation($"📦 Request Suscripción: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(
                    "https://api.mercadopago.com/preapproval", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"✅ Suscripción creada: {responseContent}");
                    
                    var subscriptionResponse = JsonSerializer.Deserialize<SubscriptionResponse>(responseContent);
                    
                    // Actualizar el plan con el ID de MercadoPago
                    if (string.IsNullOrEmpty(plan.MercadoPagoPlanId))
                    {
                        plan.MercadoPagoPlanId = mercadoPagoPlanId;
                        await _context.SaveChangesAsync();
                    }

                    // Obtener URL de pago
                    var url = _isSandbox ? 
                        subscriptionResponse.sandbox_init_point ?? subscriptionResponse.init_point : 
                        subscriptionResponse.init_point;

                    _logger.LogInformation($"🎯 URL de suscripción: {url}");
                    return url;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"❌ Error creando suscripción: {errorContent}");
                    throw new Exception($"Error creando suscripción: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error en CreateSubscription");
                throw;
            }
        }

        // ✅ CREAR O OBTENER PLAN EN MERCADO PAGO
        private async Task<string> GetOrCreateMercadoPagoPlan(Plan plan)
        {
            // Si ya tiene un ID, usarlo
            if (!string.IsNullOrEmpty(plan.MercadoPagoPlanId))
            {
                _logger.LogInformation($"✅ Usando plan existente: {plan.MercadoPagoPlanId}");
                return plan.MercadoPagoPlanId;
            }

            _logger.LogInformation($"🆕 Creando nuevo plan en MercadoPago: {plan.Nombre}");

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

            var planRequest = new
            {
                reason = plan.Nombre,
                auto_recurring = new
                {
                    frequency = 1,
                    frequency_type = "months",
                    transaction_amount = (double)plan.Precio,
                    currency_id = "PEN",
                    billing_day = DateTime.UtcNow.Day,
                    billing_day_proportional = true
                },
                payment_methods_allowed = new
                {
                    payment_types = new object[] { new { } },
                    payment_methods = new object[] { new { } }
                },
                back_url = "https://www.tusitio.com"
            };

            var json = JsonSerializer.Serialize(planRequest, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });

            _logger.LogInformation($"📦 Request Plan: {json}");

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(
                "https://api.mercadopago.com/preapproval_plan", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var planResponse = JsonSerializer.Deserialize<PlanResponse>(responseContent);
                
                _logger.LogInformation($"✅ Plan creado: {planResponse.id}");
                return planResponse.id;
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"❌ Error creando plan: {errorContent}");
                throw new Exception($"Error creando plan: {errorContent}");
            }
        }

        // ✅ MÉTODO PARA PAGOS ÚNICOS (como fallback)
        public async Task<string> CreatePreference(Subscripcion subscripcion, Usuario user, string returnUrl, string webhookUrl)
        {
            _logger.LogInformation("=== CREANDO PREFERENCIA DE PAGO ÚNICO ===");
            
            try
            {
                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);

                var preferenceRequest = new
                {
                    items = new[]
                    {
                        new
                        {
                            title = subscripcion.Plan?.Nombre ?? "Suscripción",
                            quantity = 1,
                            currency_id = "PEN",
                            unit_price = (double)(subscripcion.Plan?.Precio ?? 39.90m)
                        }
                    },
                    payer = new
                    {
                        email = user.Email
                    },
                    back_urls = new
                    {
                        success = returnUrl
                    },
                    external_reference = subscripcion.Id.ToString()
                };

                var json = JsonSerializer.Serialize(preferenceRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(
                    "https://api.mercadopago.com/checkout/preferences", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var preferenceResponse = JsonSerializer.Deserialize<PreferenceResponse>(responseContent);
                    
                    var url = _isSandbox ? 
                        preferenceResponse.sandbox_init_point ?? preferenceResponse.init_point : 
                        preferenceResponse.init_point;
                    
                    _logger.LogInformation($"✅ URL de pago único: {url}");
                    return url;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en CreatePreference");
                throw;
            }
        }

        public async Task<bool> ProcessWebhookNotification(Dictionary<string, object> data)
        {
            _logger.LogInformation("Webhook recibido: " + JsonSerializer.Serialize(data));
            return true;
        }

        // Clases para deserializar respuestas
        private class PlanResponse
        {
            public string id { get; set; }
        }

        private class SubscriptionResponse
        {
            public string id { get; set; }
            public string init_point { get; set; }
            public string sandbox_init_point { get; set; }
        }

        private class PreferenceResponse
        {
            public string id { get; set; }
            public string init_point { get; set; }
            public string sandbox_init_point { get; set; }
        }
    }
}