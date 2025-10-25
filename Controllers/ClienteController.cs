using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PROYECTOMOVIE.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace PROYECTOMOVIE.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ILogger<ClienteController> _logger;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public ClienteController(
            ILogger<ClienteController> logger,
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Página principal del cliente - REDIRIGE A PLANES SI NO TIENE PLAN
        public IActionResult Index()
        {
            try
            {
                // Verificar si el usuario tiene un plan seleccionado (usando Session)
                var planSeleccionado = HttpContext.Session?.GetString("PlanSeleccionado");
                
                if (User.Identity?.IsAuthenticated == true && string.IsNullOrEmpty(planSeleccionado))
                {
                    _logger.LogInformation("Usuario sin plan - Redirigiendo a Planes");
                    return RedirectToAction("Planes", "Cliente");
                }

                _logger.LogInformation("Accediendo a Cliente/Index - Vista principal");
                
                // Mostrar mensaje temporal si acaba de seleccionar un plan
                if (TempData["MensajeExito"] != null)
                {
                    ViewBag.MensajeExito = TempData["MensajeExito"];
                    ViewBag.PlanSeleccionado = planSeleccionado;
                }
                
                return View();
            }
            catch
            {
                // Si hay error con Session, redirigir a Planes por seguridad
                if (User.Identity?.IsAuthenticated == true)
                {
                    return RedirectToAction("Planes", "Cliente");
                }
                return View();
            }
        }

        // === LOGIN ===
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Limpiar session al mostrar el login
            HttpContext.Session?.Remove("PlanSeleccionado");
            
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Login exitoso - Redirigiendo a Planes");
                    
                    // Limpiar session al hacer login
                    HttpContext.Session?.Remove("PlanSeleccionado");
                    
                    // ✅ REDIRECCIÓN DIRECTA A PLANES después del login
                    return RedirectToAction("Planes", "Cliente");
                }
            }

            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Limpiar todo al hacer logout
            HttpContext.Session?.Clear();
            TempData.Clear();
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Cliente");
        }

        // === MOSTRAR VISTA DE PLANES DESPUÉS DEL LOGIN ===
        [HttpGet]
        [Authorize]
        public IActionResult Planes()
        {
            _logger.LogInformation("Accediendo a Cliente/Planes");
            return View();
        }

        // === ACCIÓN AL ELEGIR UN PLAN ===
        [HttpPost]
        [Authorize]
        public IActionResult SeleccionarPlan(string plan)
        {
            _logger.LogInformation($"✅ Plan seleccionado: {plan}");
    
            // Guarda el plan en SESSION (persistente)
            HttpContext.Session?.SetString("PlanSeleccionado", plan);
            
            // Mensaje temporal para mostrar una sola vez
            TempData["MensajeExito"] = $"¡Plan {plan} seleccionado correctamente!";

            _logger.LogInformation($"✅ Redirigiendo a Cliente/Index...");
    
            // ✅ Redirige al Cliente/Index
            return RedirectToAction("Index", "Cliente");
        }

        // === PERFIL DEL USUARIO ===
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Perfil()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Cliente");

            var model = new EditarPerfilViewModel
            {
                Nombre = user.Nombre ?? string.Empty,
                Apellido = user.Apellido ?? string.Empty,
                Email = user.Email ?? string.Empty
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(EditarPerfilViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Cliente");

            user.Nombre = model.Nombre;
            user.Apellido = model.Apellido;
            user.Email = model.Email;
            user.UserName = model.Email;

            var result = await _userManager.UpdateAsync(user);

            if (!string.IsNullOrEmpty(model.PasswordNueva))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var cambio = await _userManager.ResetPasswordAsync(user, token, model.PasswordNueva);
                if (!cambio.Succeeded)
                {
                    ModelState.AddModelError("", "Error al cambiar contraseña.");
                    return View(model);
                }
            }

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                ViewBag.Mensaje = "Perfil actualizado correctamente.";
            }
            else
            {
                ModelState.AddModelError("", "No se pudo actualizar el perfil.");
            }

            return View(model);
        }
    }

    // === VIEWMODEL ===
    public class EditarPerfilViewModel
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PasswordNueva { get; set; }
    }
}