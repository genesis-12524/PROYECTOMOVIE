using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PROYECTOMOVIE.Models;

namespace PROYECTOMOVIE.interfaze
{
    public interface IMercadoPagoService
    {

        // ✅ Método principal para suscripciones recurrentes
        Task<string> CreateSubscription(Plan plan, Usuario user, string returnUrl);
    
        // ✅ Método alternativo para pagos únicos
        Task<string> CreatePreference(Subscripcion subscripcion, Usuario user, string returnUrl, string webhookUrl);
    
        Task<bool> ProcessWebhookNotification(Dictionary<string, object> data);

    }
}