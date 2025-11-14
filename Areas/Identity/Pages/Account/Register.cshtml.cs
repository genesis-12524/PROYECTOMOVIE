// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using PROYECTOMOVIE.Models;
using PROYECTOMOVIE.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PROYECTOMOVIE.interfaze;

namespace PROYECTOMOVIE.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly IUserStore<Usuario> _userStore;
        private readonly IUserEmailStore<Usuario> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private readonly IMercadoPagoService _mercadoPagoService;

        public RegisterModel(
            UserManager<Usuario> userManager,
            IUserStore<Usuario> userStore,
            SignInManager<Usuario> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IMercadoPagoService mercadoPagoService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _context = context;
            _mercadoPagoService = mercadoPagoService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "El nombre es obligatorio")]
            [StringLength(30, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; }

            [Required(ErrorMessage = "El apellido es obligatorio")]
            [StringLength(50, ErrorMessage = "El apellido no puede exceder 50 caracteres")]
            [Display(Name = "Apellido")]
            public string Apellido { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Correo")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} y máximo {1} caracteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Contraseña")]
            [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Fecha de Registro")]
            public DateTime FechaRegistro { get; set; } = DateTime.Now;

            [Required(ErrorMessage = "Debe seleccionar un plan")]
            [Display(Name = "Plan de Suscripción")]
            public int PlanId { get; set; }

            public List<Plan> Planes { get; set; } = new List<Plan>();
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            
            if (Input == null)
            {
                Input = new InputModel();
            }

            Input.Planes = await _context.Planes.Where(p => p.Activo).ToListAsync();
            _logger.LogInformation($"Planes cargados: {Input.Planes?.Count ?? 0}");
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            Input.Planes = await _context.Planes.Where(p => p.Activo).ToListAsync();

            if (ModelState.IsValid)
            {
                // Validar que el plan existe y está activo
                var planSeleccionado = await _context.Planes
                    .FirstOrDefaultAsync(p => p.Id == Input.PlanId && p.Activo);
                    
                if (planSeleccionado == null)
                {
                    ModelState.AddModelError("Input.PlanId", "Plan seleccionado no válido o no disponible");
                    return Page();
                }

                // Verificar si el email ya existe
                var existingUser = await _userManager.FindByEmailAsync(Input.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Input.Email", "Ya existe un usuario con este email.");
                    return Page();
                }

                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                user.Nombre = Input.Nombre;
                user.Apellido = Input.Apellido;
                user.FechaRegistro = DateTime.Now;

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Usuario creado exitosamente.");

                    // Crear rol Cliente si no existe
                    if (!await _roleManager.RoleExistsAsync("Cliente"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Cliente"));
                    }

                    await _userManager.AddToRoleAsync(user, "Cliente");

                    // CREAR SUSCRIPCIÓN EN LA BASE DE DATOS
                    var subscription = new Subscripcion
                    {
                        UserId = user.Id,
                        PlanId = Input.PlanId,
                        Status = "pending",
                        FechaInicio = DateTime.UtcNow,
                        FechaCreacion = DateTime.UtcNow,
                        SubscriptionIdMercadoPago = ""
                    };

                    _context.Subscripciones.Add(subscription);
                    await _context.SaveChangesAsync();

                    // ✅ CORREGIDO: NUEVO ENFOQUE - SUSCRIPCIÓN RECURRENTE
                    try
                    {
                        var baseUrl = $"{Request.Scheme}://{Request.Host}";
                        var returnUrlMercadoPago = $"{baseUrl}/Identity/MercadoPago/Resultado";

                        _logger.LogInformation("=== INICIANDO SUSCRIPCIÓN RECURRENTE ===");
                        _logger.LogInformation($"Plan: {planSeleccionado.Nombre}, Usuario: {user.Email}");
                        _logger.LogInformation($"Return URL: {returnUrlMercadoPago}");

                        // ✅ GUARDAR INFORMACIÓN PARA USAR DESPUÉS
                        HttpContext.Session.SetInt32("CurrentSubscriptionId", subscription.Id);
                        HttpContext.Session.SetString("CurrentUserId", user.Id);
                        TempData["CurrentSubscriptionId"] = subscription.Id;
                        TempData["CurrentUserId"] = user.Id;

                        // ✅ PRIMERO INTENTAR SUSCRIPCIÓN RECURRENTE
                        _logger.LogInformation("🔄 Intentando crear SUSCRIPCIÓN RECURRENTE...");
                        var mercadoPagoUrl = await _mercadoPagoService.CreateSubscription(
                            planSeleccionado, user, returnUrlMercadoPago);

                        _logger.LogInformation($"✅ URL de suscripción recurrente: {mercadoPagoUrl}");

                        if (string.IsNullOrEmpty(mercadoPagoUrl))
                        {
                            throw new Exception("MercadoPago no devolvió una URL válida para suscripción recurrente");
                        }

                        // ✅ GUARDAR INFORMACIÓN TEMPORAL
                        TempData["SubscriptionId"] = subscription.Id;
                        TempData["UserId"] = user.Id;
                        TempData["UserEmail"] = user.Email;
                        TempData["UserName"] = $"{user.Nombre} {user.Apellido}";
                        TempData["PlanId"] = Input.PlanId;
                        TempData["PlanNombre"] = planSeleccionado.Nombre;
                        TempData["PlanPrecio"] = planSeleccionado.Precio;
                        TempData["PlanDescripcion"] = planSeleccionado.Descripcion;
                        TempData["TipoPago"] = "recurrente"; // ✅ Indicar que es suscripción recurrente

                        _logger.LogInformation($"🔀 REDIRIGIENDO A MERCADO PAGO (Suscripción): {mercadoPagoUrl}");

                        // ✅ REDIRIGIR A MERCADO PAGO
                        return Redirect(mercadoPagoUrl);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "❌ Error con suscripción recurrente, intentando pago único...");
                        
                        // ✅ FALLBACK: INTENTAR PAGO ÚNICO
                        try
                        {
                            var baseUrl = $"{Request.Scheme}://{Request.Host}";
                            var returnUrlMercadoPago = $"{baseUrl}/Identity/MercadoPago/Resultado";
                            var webhookUrl = $"{baseUrl}/Identity/MercadoPago/Webhook";

                            _logger.LogInformation("🔄 Intentando PAGO ÚNICO como fallback...");
                            
                            var mercadoPagoUrl = await _mercadoPagoService.CreatePreference(
                                subscription, user, returnUrlMercadoPago, webhookUrl);

                            _logger.LogInformation($"✅ URL de pago único: {mercadoPagoUrl}");

                            if (string.IsNullOrEmpty(mercadoPagoUrl))
                            {
                                throw new Exception("MercadoPago no devolvió una URL válida para pago único");
                            }

                            TempData["TipoPago"] = "unico"; // ✅ Indicar que es pago único

                            _logger.LogInformation($"🔀 REDIRIGIENDO A MERCADO PAGO (Pago Único): {mercadoPagoUrl}");
                            return Redirect(mercadoPagoUrl);
                        }
                        catch (Exception fallbackEx)
                        {
                            _logger.LogError(fallbackEx, "❌ Error también con pago único");
                            
                            // Si fallan ambos métodos, marcar suscripción como fallida
                            subscription.Status = "failed";
                            await _context.SaveChangesAsync();

                            ModelState.AddModelError(string.Empty, 
                                "Error al procesar el pago. Tu cuenta fue creada pero la suscripción no pudo activarse. Por favor contacta con soporte.");
                            return Page();
                        }
                    }
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            Input.Planes = await _context.Planes.Where(p => p.Activo).ToListAsync();
            return Page();
        }

        private Usuario CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Usuario>();
            }
            catch
            {
                throw new InvalidOperationException($"No se puede crear una instancia de '{nameof(Usuario)}'. " +
                    $"Asegúrate de que '{nameof(Usuario)}' no es una clase abstracta y tiene un constructor sin parámetros.");
            }
        }

        private IUserEmailStore<Usuario> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("El UI por defecto requiere un user store con soporte de email.");
            }
            return (IUserEmailStore<Usuario>)_userStore;
        }
    }
}