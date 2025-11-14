using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PROYECTOMOVIE.Data;
using PROYECTOMOVIE.Models;

namespace PROYECTOMOVIE.Controllers
{
    [Authorize]
    public class ListaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;

        public ListaController(ApplicationDbContext context, UserManager<Usuario> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Lista/Index - Mostrar todas las listas del usuario
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var listas = await _context.DataLista
                .Where(l => l.UsuarioId == userId)
                .Include(l => l.ListaPeliculas!)
                    .ThenInclude(lp => lp.Pelicula)
                .OrderByDescending(l => l.FechaCreacion)
                .ToListAsync();

            return View(listas);
        }

        // GET: Lista/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Lista/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Nombre,Descripcion,EsPublica")] Lista lista)
        {
            try
            {
                // Remover el error de UsuarioId del ModelState
                ModelState.Remove("UsuarioId");
        
                Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
                Console.WriteLine($"Nombre recibido: {lista.Nombre}");
        
                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"❌ Error de validación: {error.ErrorMessage}");
                    }
                    return View(lista);
                }

                var userId = _userManager.GetUserId(User);
                Console.WriteLine($"UserId obtenido: {userId}");
        
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError("", "Usuario no autenticado");
                    return View(lista);
                }

                lista.UsuarioId = userId;
                lista.FechaCreacion = DateTime.Now;
        
                Console.WriteLine($"✅ Guardando lista: {lista.Nombre}");
        
                _context.Add(lista);
                await _context.SaveChangesAsync();
        
                Console.WriteLine($"✅ Lista guardada con ID: {lista.Id}");
        
                TempData["Message"] = "Lista creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR al crear lista: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                ModelState.AddModelError("", "Error al crear la lista: " + ex.Message);
                return View(lista);
            }
        }

        // GET: Lista/Detalle/5
        public async Task<IActionResult> Detalle(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var lista = await _context.DataLista
                .Include(l => l.ListaPeliculas!)
                    .ThenInclude(lp => lp.Pelicula)
                .FirstOrDefaultAsync(l => l.Id == id && l.UsuarioId == userId);

            if (lista == null) return NotFound();

            // Ordenar películas por orden y fecha
            if (lista.ListaPeliculas != null)
            {
                lista.ListaPeliculas = lista.ListaPeliculas
                    .OrderBy(lp => lp.Orden)
                    .ThenByDescending(lp => lp.FechaAgregada)
                    .ToList();
            }

            return View(lista);
        }

        // POST: Lista/AgregarPelicula - Agregar película a una lista (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarPelicula(int listaId, int peliculaId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var lista = await _context.DataLista
                    .FirstOrDefaultAsync(l => l.Id == listaId && l.UsuarioId == userId);

                if (lista == null)
                    return Json(new { success = false, message = "Lista no encontrada" });

                // Verificar si la película ya existe en la lista
                var existe = await _context.DataListaPelicula
                    .AnyAsync(lp => lp.ListaId == listaId && lp.PeliculaId == peliculaId);

                if (existe)
                    return Json(new { success = false, message = "La película ya está en esta lista" });

                // Obtener el siguiente número de orden
                var maxOrden = await _context.DataListaPelicula
                    .Where(lp => lp.ListaId == listaId)
                    .Select(lp => lp.Orden)
                    .DefaultIfEmpty(0)
                    .MaxAsync();

                var listaPelicula = new ListaPelicula
                {
                    ListaId = listaId,
                    PeliculaId = peliculaId,
                    FechaAgregada = DateTime.Now,
                    Orden = maxOrden + 1
                };

                _context.DataListaPelicula.Add(listaPelicula);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Película agregada a la lista exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al agregar la película: " + ex.Message });
            }
        }

        // POST: Lista/EliminarPelicula - Quitar película de una lista (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPelicula(int listaId, int peliculaId)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var lista = await _context.DataLista
                    .FirstOrDefaultAsync(l => l.Id == listaId && l.UsuarioId == userId);

                if (lista == null)
                    return Json(new { success = false, message = "Lista no encontrada" });

                var listaPelicula = await _context.DataListaPelicula
                    .FirstOrDefaultAsync(lp => lp.ListaId == listaId && lp.PeliculaId == peliculaId);

                if (listaPelicula == null)
                    return Json(new { success = false, message = "Película no encontrada en la lista" });

                _context.DataListaPelicula.Remove(listaPelicula);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Película eliminada de la lista" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar: " + ex.Message });
            }
        }

        // GET: Lista/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null) return NotFound();

            var userId = _userManager.GetUserId(User);
            var lista = await _context.DataLista
                .FirstOrDefaultAsync(l => l.Id == id && l.UsuarioId == userId);

            if (lista == null) return NotFound();

            return View(lista);
        }

        // POST: Lista/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Lista lista)
        {
            if (id != lista.Id) return NotFound();

            var userId = _userManager.GetUserId(User);
            var listaExistente = await _context.DataLista
                .FirstOrDefaultAsync(l => l.Id == id && l.UsuarioId == userId);

            if (listaExistente == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    listaExistente.Nombre = lista.Nombre;
                    listaExistente.Descripcion = lista.Descripcion;
                    listaExistente.EsPublica = lista.EsPublica;

                    _context.Update(listaExistente);
                    await _context.SaveChangesAsync();
                    
                    TempData["Message"] = "Lista actualizada exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListaExists(lista.Id))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(lista);
        }

        // POST: Lista/Eliminar/5 (AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var lista = await _context.DataLista
                    .FirstOrDefaultAsync(l => l.Id == id && l.UsuarioId == userId);

                if (lista == null)
                    return Json(new { success = false, message = "Lista no encontrada" });

                _context.DataLista.Remove(lista);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Lista eliminada exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error al eliminar: " + ex.Message });
            }
        }

        // GET: Lista/ObtenerListasUsuario - Obtener listas para modal (AJAX)
        [HttpGet]
        public async Task<IActionResult> ObtenerListasUsuario()
        {
            var userId = _userManager.GetUserId(User);
            var listas = await _context.DataLista
                .Where(l => l.UsuarioId == userId)
                .OrderBy(l => l.Nombre)
                .Select(l => new { l.Id, l.Nombre })
                .ToListAsync();

            return Json(listas);
        }

        private bool ListaExists(int id)
        {
            return _context.DataLista.Any(e => e.Id == id);
        }
    }
}