using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROYECTOMOVIE.Data;
using PROYECTOMOVIE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PROYECTOMOVIE.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger, 
            UserManager<Usuario> userManager, SignInManager<Usuario> signInManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Peliculas()
        {
            return View();
        }

        public IActionResult ListarUsuarios()
        {
            var usuarios = _context.Users
            .Select(u => new Usuario
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Email = u.Email,
                FechaRegistro = u.FechaRegistro
            }).ToList();

            return View(usuarios);
        }

        // GET: Perfil del usuario
        [HttpGet]
        public async Task<IActionResult> Perfil()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"No se pudo cargar el usuario con ID '{_userManager.GetUserId(User)}'.");
            }

            var model = new PerfilVistaUsuario
            {
                Id = user.Id,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Email = user.Email,
                NombredeUsuario = user.UserName,
                FechaRegistro = user.FechaRegistro
            };

            // Limpiar ModelState para la vista GET
            ModelState.Clear();
            return View(model);
        }

        // POST: Perfil - Maneja ambos formularios (actualizar perfil y cambiar contraseña)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(PerfilVistaUsuario model, string actionType)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"No se pudo cargar el usuario con ID '{_userManager.GetUserId(User)}'.");
            }

            // Determinar qué acción ejecutar basado en el botón presionado
            if (actionType == "updateProfile")
            {
                return await ActualizarPerfil(model, user);
            }
            else if (actionType == "changePassword")
            {
                return await CambiarPassword(model, user);
            }

            // Si no se reconoce la acción, recargar la vista
            return await RecargarPerfil(model, user);
        }

        private async Task<IActionResult> ActualizarPerfil(PerfilVistaUsuario model, Usuario user)
        {
            // Limpiar ModelState antes de validar
            ModelState.Clear();

            // Validar solo los campos del perfil
            bool hasErrors = false;

            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("Email", "El correo electrónico es requerido.");
                hasErrors = true;
            }

            if (string.IsNullOrEmpty(model.Nombre))
            {
                ModelState.AddModelError("Nombre", "El nombre es requerido.");
                hasErrors = true;
            }

            if (string.IsNullOrEmpty(model.Apellido))
            {
                ModelState.AddModelError("Apellido", "El apellido es requerido.");
                hasErrors = true;
            }

            if (hasErrors)
            {
                await CargarDatosUsuario(model, user);
                TempData["ErrorMessage"] = "Por favor, corrige los errores en el formulario.";
                return View("Perfil", model);
            }

            try
            {
                // Actualizar información básica
                user.Nombre = model.Nombre;
                user.Apellido = model.Apellido;
                user.Email = model.Email;
                user.UserName = model.Email; // Mantener username igual al email

                var result = await _userManager.UpdateAsync(user);
                
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Perfil actualizado correctamente.";
                    _logger.LogInformation($"Perfil actualizado para el usuario: {user.Email}");
                    
                    // Recargar datos después de actualizar
                    await CargarDatosUsuario(model, user);
                    ModelState.Clear();
                    return View("Perfil", model);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    TempData["ErrorMessage"] = "Error al actualizar el perfil.";
                    _logger.LogWarning($"Error al actualizar perfil para {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al actualizar el perfil: " + ex.Message);
                TempData["ErrorMessage"] = "Error al actualizar el perfil.";
                _logger.LogError(ex, $"Error al actualizar perfil para {user.Email}");
            }

            await CargarDatosUsuario(model, user);
            return View("Perfil", model);
        }

        private async Task<IActionResult> CambiarPassword(PerfilVistaUsuario model, Usuario user)
        {
            // Limpiar ModelState antes de validar
            ModelState.Clear();

            // Validar solo los campos de contraseña
            bool hasErrors = false;

            if (string.IsNullOrEmpty(model.CurrentPassword))
            {
                ModelState.AddModelError("CurrentPassword", "La contraseña actual es requerida.");
                hasErrors = true;
            }

            if (string.IsNullOrEmpty(model.NewPassword))
            {
                ModelState.AddModelError("NewPassword", "La nueva contraseña es requerida.");
                hasErrors = true;
            }
            else if (model.NewPassword.Length < 6)
            {
                ModelState.AddModelError("NewPassword", "La contraseña debe tener al menos 6 caracteres.");
                hasErrors = true;
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Las contraseñas no coinciden.");
                hasErrors = true;
            }

            if (hasErrors)
            {
                await CargarDatosUsuario(model, user);
                TempData["ErrorMessage"] = "Por favor, corrige los errores en el formulario de contraseña.";
                return View("Perfil", model);
            }

            try
            {
                // Cambiar contraseña
                var changePasswordResult = await _userManager.ChangePasswordAsync(
                    user, model.CurrentPassword, model.NewPassword);

                if (changePasswordResult.Succeeded)
                {
                    // Actualizar la sesión después de cambiar la contraseña
                    await _signInManager.RefreshSignInAsync(user);
                    
                    TempData["SuccessMessage"] = "¡Contraseña cambiada exitosamente!";
                    _logger.LogInformation($"Contraseña cambiada exitosamente para el usuario: {user.Email}");
                    
                    // Recargar los datos del usuario y limpiar campos
                    await CargarDatosUsuario(model, user);
                    model.CurrentPassword = null;
                    model.NewPassword = null;
                    model.ConfirmPassword = null;
                    
                    ModelState.Clear();
                    return View("Perfil", model);
                }
                else
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        if (error.Code.Contains("PasswordMismatch"))
                        {
                            ModelState.AddModelError("CurrentPassword", "La contraseña actual es incorrecta.");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                    TempData["ErrorMessage"] = "Error al cambiar la contraseña. Verifica los datos.";
                    _logger.LogWarning($"Error al cambiar contraseña para {user.Email}: {string.Join(", ", changePasswordResult.Errors.Select(e => e.Description))}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Error al cambiar la contraseña: " + ex.Message);
                TempData["ErrorMessage"] = "Error inesperado al cambiar la contraseña.";
                _logger.LogError(ex, $"Error al cambiar contraseña para {user.Email}");
            }

            await CargarDatosUsuario(model, user);
            return View("Perfil", model);
        }

        private async Task<IActionResult> RecargarPerfil(PerfilVistaUsuario model, Usuario user)
        {
            await CargarDatosUsuario(model, user);
            ModelState.Clear();
            return View("Perfil", model);
        }

        private async Task CargarDatosUsuario(PerfilVistaUsuario model, Usuario user)
        {
            // Recargar datos del usuario desde la base de datos
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                model.Id = currentUser.Id;
                model.Nombre = currentUser.Nombre;
                model.Apellido = currentUser.Apellido;
                model.Email = currentUser.Email;
                model.NombredeUsuario = currentUser.UserName;
                model.FechaRegistro = currentUser.FechaRegistro;
            }
        }

        // POST: Eliminar Usuario con Identity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarUsuario(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    TempData["Error"] = "ID de usuario no válido";
                    return RedirectToAction(nameof(ListarUsuarios));
                }

                // Buscar usuario con Identity
                var usuario = await _userManager.FindByIdAsync(id);
                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction(nameof(ListarUsuarios));
                }

                // Verificar que no sea el usuario actual
                var currentUserId = _userManager.GetUserId(User);
                if (usuario.Id == currentUserId)
                {
                    TempData["Error"] = "No puedes eliminar tu propio usuario";
                    return RedirectToAction(nameof(ListarUsuarios));
                }

                // Eliminar usuario con Identity
                var result = await _userManager.DeleteAsync(usuario);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Usuario {usuario.Email} eliminado exitosamente");
                    TempData["Success"] = "Usuario eliminado exitosamente";
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError($"Error al eliminar usuario: {errors}");
                    TempData["Error"] = $"Error al eliminar el usuario: {errors}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario");
                TempData["Error"] = "Ocurrió un error al eliminar el usuario";
            }

            return RedirectToAction(nameof(ListarUsuarios));
        }

        [HttpGet]
        public async Task<IActionResult> GestionarVideoConfigPortada()
        {
            var model = new VideoPortadaViewModel();

            // Obtener la configuración activa desde la base de datos
            var configActiva = await _context.VideoConfigPortadas
                .Include(h => h.Pelicula)
                .Where(h => h.EstaActivo)
                .FirstOrDefaultAsync();

            if (configActiva?.Pelicula != null)
            {
                var pelicula = configActiva.Pelicula;
                model.PeliculaSeleccionadaId = pelicula.Id;
                model.NombrePelicula = pelicula.Nombre_Peli;
                model.DescripcionPelicula = pelicula.Descripción;
                model.TrailerUrl = pelicula.Video_Trailer;
                model.TieneVideoConfigurado = true;
            }

            // Cargar películas disponibles
            model.PeliculasDisponibles = await _context.Peliculas
                .Where(p => !string.IsNullOrEmpty(p.Video_Trailer))
                .ToListAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfigurarVideoPortada(int peliculaId)
        {
            // Verificar que la película existe y tiene trailer
            var pelicula = await _context.Peliculas
                .FirstOrDefaultAsync(p => p.Id == peliculaId && !string.IsNullOrEmpty(p.Video_Trailer));

            if (pelicula == null)
            {
                TempData["Error"] = "La película seleccionada no existe o no tiene trailer disponible";
                return RedirectToAction("GestionarVideoConfigPortada");
            }

            // Desactivar todas las configuraciones anteriores
            var configsActivas = await _context.VideoConfigPortadas
                .Where(h => h.EstaActivo)
                .ToListAsync();

            foreach (var config in configsActivas)
            {
                config.EstaActivo = false;
            }

            // Crear nueva configuración
            var nuevaConfig = new VideoConfigPortada
            {
                PeliculaId = peliculaId,
                FechaConfiguracion = DateTime.Now,
                EstaActivo = true
            };

            _context.VideoConfigPortadas.Add(nuevaConfig);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Video de Portada configurado con: {pelicula.Nombre_Peli}";
            return RedirectToAction("GestionarVideoConfigPortada");
        }
        
        [HttpPost]
        public async Task<IActionResult> EliminarVideoPortada()
        {
            // Desactivar todas las configuraciones
            var configsActivas = await _context.VideoConfigPortadas
                .Where(h => h.EstaActivo)
                .ToListAsync();
            
            foreach (var config in configsActivas)
            {
                config.EstaActivo = false;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Video de Portada eliminado correctamente";
            return RedirectToAction("GestionarVideoConfigPortada");
        }
    }
}