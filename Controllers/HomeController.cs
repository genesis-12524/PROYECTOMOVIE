    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using PROYECTOMOVIE.Models;
    using Microsoft.EntityFrameworkCore;
    using PROYECTOMOVIE.Data;
    using System.Linq;

    namespace PROYECTOMOVIE.Controllers;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // --- 1. ACCIÓN INDEX (PÁGINA PRINCIPAL) ---
        // Esta es la acción para la página principal (http://localhost:5162/)
        // NO LLEVA PARÁMETROS.
        // Carga solo 10 películas y 10 series (como la tenías al inicio).
        public async Task<IActionResult> Index()
        {
            var viewModel = new VerPorLista();

            viewModel.Peliculas = await _context.Peliculas
                                        .OrderByDescending(p => p.Fecha_Publicada)
                                        .Take(10)
                                        .ToListAsync();
            
            viewModel.Series = await _context.Series 
                                        .OrderByDescending(s => s.Fecha_Publicada)
                                        .Take(10)
                                        .ToListAsync();
            
            // La página principal también podría necesitar las categorías
            // (si es que tienes filtros en el Index también)
            // Si no los tienes, puedes comentar esta línea.
            viewModel.Categorias = await _context.Categoria.ToListAsync();

            return View(viewModel);
        }


        // --- 2. ACCIÓN PELIS (PÁGINA DE PELÍCULAS) ---
        // Esta es la acción para la página de "Películas" (http://localhost:5162/Home/Pelis)
        // SÍ LLEVA EL PARÁMETRO (int? categoriaId) para los filtros.
        // Carga TODAS las películas (o las filtradas).
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
        
        // --- 3. RESTO DE ACCIONES ---
        
        // (Asegúrate de que la acción 'Series' tenga la misma lógica de filtro que 'Pelis')
        public async Task<IActionResult> Series(int? categoriaId)
        {
            var viewModel = new VerPorLista();

            if (categoriaId.HasValue && categoriaId > 0)
            {
                var categoriaFiltrada = await _context.Categoria
                    .Include(c => c.Series) // <-- CAMBIA A SERIES
                    .FirstOrDefaultAsync(c => c.Id == categoriaId.Value);

                if (categoriaFiltrada != null && categoriaFiltrada.Series != null)
                {
                    viewModel.Series = categoriaFiltrada.Series
                                                .OrderBy(s => s.Nombre_Serie) // <-- CAMBIA A SERIES
                                                .ToList();
                }
            }
            else
            {
                viewModel.Series = await _context.Series
                                            .OrderBy(s => s.Nombre_Serie) // <-- CAMBIA A SERIES
                                            .ToListAsync();
            }

            viewModel.Categorias = await _context.Categoria
                                        .OrderBy(c => c.Nombre)
                                        .ToListAsync();

            ViewData["SelectedCategoriaId"] = categoriaId; 
            
            return View(viewModel); // Necesitas una vista 'Series.cshtml'
        }

        public async Task<IActionResult> Buscar(string terminoBusqueda)
        {
            var viewModel = new VerPorLista { TerminoBusqueda = terminoBusqueda };

            if (!string.IsNullOrEmpty(terminoBusqueda))
            {
                string terminoLower = terminoBusqueda.ToLower();
                viewModel.Peliculas = await _context.Peliculas
                    .Where(p => p.Nombre_Peli != null && p.Nombre_Peli.ToLower().Contains(terminoLower))
                    .ToListAsync();
                viewModel.Series = await _context.Series
                    .Where(s => s.Nombre_Serie != null && s.Nombre_Serie.ToLower().Contains(terminoLower))
                    .ToListAsync();
            }
            else
            {
                // Evita cargar todo si la búsqueda está vacía, mejor redirige
                return RedirectToAction("Index"); // O a "Pelis" si lo prefieres
            }
            
            return View("ResultadosBusqueda", viewModel);
        }

        public async Task<IActionResult> Aleatorio()
        {
            var count = await _context.Peliculas.CountAsync();
            if (count == 0) return RedirectToAction("Index");
            
            var random = new Random();
            int index = random.Next(count);
            
            var randomPelicula = await _context.Peliculas.Skip(index).FirstOrDefaultAsync();
            if (randomPelicula == null) return RedirectToAction("Index");

            return RedirectToAction("PeliculaDetalle", new { id = randomPelicula.Id }); 
        }

        public async Task<IActionResult> PeliculaDetalle(int id)
        {
            var pelicula = await _context.Peliculas.FindAsync(id);
            if (pelicula == null) return NotFound();
            return View("PeliculaDetalle", pelicula); // Necesitas 'PeliculaDetalle.cshtml'
        }

        public async Task<IActionResult> SerieDetalle(int id)
        {
            var serie = await _context.Series.FindAsync(id);
            if (serie == null) return NotFound();
            return View("SerieDetalle", serie); // Necesitas 'SerieDetalle.cshtml'
        }
        
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }