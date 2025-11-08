using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PROYECTOMOVIE.Data;
using PROYECTOMOVIE.Models;

namespace PROYECTOMOVIE.Controllers
{
    [Authorize] // 🔒 Solo usuarios autenticados podrán acceder
    public class ClienteController : Controller
    {
        private readonly ILogger<ClienteController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Usuario> _userManager;
        private readonly SignInManager<Usuario> _signInManager;

        public ClienteController(
            ILogger<ClienteController> logger,
            ApplicationDbContext context,
            UserManager<Usuario> userManager,
            SignInManager<Usuario> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // === VISTA PRINCIPAL DEL CLIENTE ===
        public IActionResult Index()
        {
            var viewModel = new VerPorLista
            {
                Peliculas = _context.Peliculas
                    .OrderByDescending(p => p.Fecha_Publicada)
                    .Take(10)
                    .ToList(),
                Series = _context.Series
                    .OrderByDescending(s => s.Fecha_Publicada)
                    .Take(10)
                    .ToList()
            };

            return View(viewModel); // 🔹 Muestra Views/Cliente/Index.cshtml
        }

        // === LOGIN ===
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
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
                    _logger.LogInformation($"✅ Login exitoso de {user.Email}");
                    // 🔹 Redirige al "Home privado" → Cliente/Index
                    return RedirectToAction("Index", "Cliente");
                }
            }

            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View();
        }

        // === REGISTRO ===
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(Usuario model, string password)
        {
            if (ModelState.IsValid)
            {
                var user = new Usuario
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Nombre = model.Nombre,
                    Apellido = model.Apellido
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation($"🟢 Nuevo usuario registrado: {user.Email}");
                    // 🔹 Después del registro, también va al “Home privado”
                    return RedirectToAction("Index", "Cliente");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        // === LOGOUT ===
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("🔴 Usuario cerró sesión.");
            return RedirectToAction("Index", "Home");
        }

        // === FUNCIONALIDADES INTERNAS ===

        public async Task<IActionResult> Buscar(string terminoBusqueda)
        {
            var peliculasQuery = from peli in _context.Peliculas select peli;
            var seriesQuery = from serie in _context.Series select serie;

            var viewModel = new VerPorLista
            {
                TerminoBusqueda = terminoBusqueda
            };

            if (!string.IsNullOrEmpty(terminoBusqueda))
            {
                string terminoEnMinusculas = terminoBusqueda.ToLower();

                peliculasQuery = peliculasQuery.Where(
                    p => p.Nombre_Peli != null && p.Nombre_Peli.ToLower().Contains(terminoEnMinusculas)
                );

                seriesQuery = seriesQuery.Where(
                    s => s.Nombre_Serie != null && s.Nombre_Serie.ToLower().Contains(terminoEnMinusculas)
                );
            }

            viewModel.Peliculas = await peliculasQuery.ToListAsync();
            viewModel.Series = await seriesQuery.ToListAsync();

            return View("ResultadosBusqueda", viewModel);
        }

        public async Task<IActionResult> Aleatorio()
        {
            var count = await _context.Peliculas.CountAsync();
            if (count == 0)
                return RedirectToAction("Index");

            var random = new Random();
            int index = random.Next(count);
            var randomPelicula = await _context.Peliculas.Skip(index).FirstOrDefaultAsync();

            if (randomPelicula == null)
                return RedirectToAction("Index");

            return RedirectToAction("PeliculaDetalle", new { id = randomPelicula.Id });
        }

        public async Task<IActionResult> PeliculaDetalle(int id)
        {
            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula == null)
                return NotFound();

            return View("PeliculaDetalle", pelicula);
        }

        public async Task<IActionResult> SerieDetalle(int id)
        {
            var serie = await _context.Series.FindAsync(id);
            if (serie == null)
                return NotFound();

            return View("SerieDetalle", serie);
        }

        // === ACCIÓN PELIS (PÁGINA DE PELÍCULAS CON FILTROS) ===
        public async Task<IActionResult> Pelis(int? categoriaId)
        {
            var viewModel = new VerPorLista();

            // --- Lógica de Películas (CON FILTRO) ---
            if (categoriaId.HasValue && categoriaId > 0)
            {
                // MODO FILTRADO:
                var categoriaFiltrada = await _context.Categoria
                    .Include(c => c.Peliculas)
                    .FirstOrDefaultAsync(c => c.Id == categoriaId.Value);

                if (categoriaFiltrada != null && categoriaFiltrada.Peliculas != null)
                {
                    viewModel.Peliculas = categoriaFiltrada.Peliculas
                                                .OrderBy(p => p.Nombre_Peli)
                                                .ToList();
                }
            }
            else
            {
                // MODO "VER TODOS":
                viewModel.Peliculas = await _context.Peliculas
                                            .OrderBy(p => p.Nombre_Peli)
                                            .ToListAsync();
            }

            // --- Lógica de Categorías (PARA LOS BOTONES) ---
            viewModel.Categorias = await _context.Categoria
                                        .OrderBy(c => c.Nombre)
                                        .ToListAsync();

            ViewData["SelectedCategoriaId"] = categoriaId;

            return View(viewModel);
        }

        // === ACCIÓN SERIES (PÁGINA DE SERIES CON FILTROS) ===
        public async Task<IActionResult> Series(int? categoriaId)
        {
            var viewModel = new VerPorLista();

            if (categoriaId.HasValue && categoriaId > 0)
            {
                var categoriaFiltrada = await _context.Categoria
                    .Include(c => c.Series)
                    .FirstOrDefaultAsync(c => c.Id == categoriaId.Value);

                if (categoriaFiltrada != null && categoriaFiltrada.Series != null)
                {
                    viewModel.Series = categoriaFiltrada.Series
                                                .OrderBy(s => s.Nombre_Serie)
                                                .ToList();
                }
            }
            else
            {
                viewModel.Series = await _context.Series
                                            .OrderBy(s => s.Nombre_Serie)
                                            .ToListAsync();
            }

            viewModel.Categorias = await _context.Categoria
                                        .OrderBy(c => c.Nombre)
                                        .ToListAsync();

            ViewData["SelectedCategoriaId"] = categoriaId;

            return View(viewModel);
        }

        // === ACCIÓN GEMINI ===
        public IActionResult Gemini()
        {
            return View();
        }

        public IActionResult ListasCliente()
        {
            return View(); // Busca la vista Views/Cliente/ListasCliente.cshtml
        }
    }
}