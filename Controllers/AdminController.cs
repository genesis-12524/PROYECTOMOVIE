using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PROYECTOMOVIE.Data;
using PROYECTOMOVIE.Models;
using Microsoft.AspNetCore.Identity;

namespace PROYECTOMOVIE.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        private readonly UserManager<Usuario> _userManager;




        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger, UserManager<Usuario> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
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

        public IActionResult Sistema()
        {
            return View();
        }

        public IActionResult Perfil()
        {
            return View();
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


    }
    
    

}