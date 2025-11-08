using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PROYECTOMOVIE.Models;
using Microsoft.EntityFrameworkCore;
using PROYECTOMOVIE.Data;
// using AspNetCoreGeneratedDocument;

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

    public IActionResult Index()
    {
        // 1. Crea una instancia del ViewModel
        var viewModel = new VerPorLista();

        // 2. Llena las listas desde la base de datos
        // (Usamos .Take(10) para no cargar 1000 películas en la página principal)
        viewModel.Peliculas = _context.Peliculas
                                .OrderByDescending(p => p.Fecha_Publicada) // Ejemplo: más nuevas primero
                                .Take(10) // Muestra solo las primeras 10
                                .ToList();
        
        viewModel.Series = _context.Series // Asumo que tu tabla se llama 'Series'
                                .OrderByDescending(s => s.Fecha_Publicada)
                                .Take(10)
                                .ToList();
        return View(viewModel);
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

    // --- Ventana ---
    public IActionResult Series()
    {
        return View();
    }

    public IActionResult Pelis()
    {
        return View();
    }

    public IActionResult Gemini()
    {
        return View();
    }

    // --- 2. AQUÍ ESTÁ LA NUEVA ACCIÓN DE BÚSQUEDA ---
    public async Task<IActionResult> Buscar(string terminoBusqueda)
    {
        // 1. Prepara las consultas base para ambas tablas
            var peliculasQuery = from peli in _context.Peliculas
                                    select peli;

            // Asumimos que tu contexto tiene una tabla llamada "Series"
            var seriesQuery = from serie in _context.Series 
                            select serie;

            // 2. Prepara el ViewModel que se enviará a la vista
            //    Esta es la VARIABLE que usaremos
            var viewModel = new VerPorLista
            {
                TerminoBusqueda = terminoBusqueda
            };

            // 3. Si el término no está vacío, filtra AMBAS consultas
            if (!string.IsNullOrEmpty(terminoBusqueda))
            {
                string terminoEnMinusculas = terminoBusqueda.ToLower();

                // Filtrar películas
                peliculasQuery = peliculasQuery.Where(
                    p => p.Nombre_Peli != null &&
                            p.Nombre_Peli.ToLower().Contains(terminoEnMinusculas)
                );

                // Filtrar series
                // ¡Importante! Asumo que la propiedad se llama 'Nombre_Serie'. 
                // Ajústala si se llama diferente en tu modelo 'Serie'.
                seriesQuery = seriesQuery.Where(
                    s => s.Nombre_Serie != null &&
                            s.Nombre_Serie.ToLower().Contains(terminoEnMinusculas)
                );
            }

            // 4. Ejecuta las consultas y llena el ViewModel
            //    (¡AQUÍ ESTÁ LA CORRECCIÓN!)
            viewModel.Peliculas = await peliculasQuery.ToListAsync();
            viewModel.Series = await seriesQuery.ToListAsync();

            // 5. Envía el ViewModel (la variable) a la vista "ResultadosBusqueda"
            //    (¡AQUÍ ESTÁ LA OTRA CORRECCIÓN!)
            return View("ResultadosBusqueda", viewModel);
    }
    
    // --- 1. NUEVA ACCIÓN PARA EL BOTÓN "MODO ALEATORIO" ---
    public async Task<IActionResult> Aleatorio()
    {
        // 1. Contamos cuántas películas hay
        var count = await _context.Peliculas.CountAsync();
        
        if (count == 0)
        {
            // Si no hay películas, regresamos al Inicio
            return RedirectToAction("Index"); 
        }

        // 2. Generamos un número aleatorio (índice)
        var random = new Random();
        int index = random.Next(count); // Un número entre 0 y (total - 1)

        // 3. Buscamos la película en ese índice
        var randomPelicula = await _context.Peliculas
                                        .Skip(index)
                                        .FirstOrDefaultAsync();

        if (randomPelicula == null)
        {
            return RedirectToAction("Index");
        }

        // 4. Redirigimos al usuario a la página de Detalles de esa película
        // (Asumiendo que tu modelo 'Pelicula' tiene una propiedad 'Id')
        // !! CORRECCIÓN SUGERIDA: 
        //    Cambié "PeliculaAleDetalle" por "PeliculaDetalle" para que coincida 
        //    con el nombre de tu acción de abajo.
        return RedirectToAction("PeliculaDetalle", new { id = randomPelicula.Id }); 
    }


    // --- 2. NUEVA ACCIÓN Y PÁGINA PARA MOSTRAR DETALLES ---
    // Esta acción responde a la URL: /Home/PeliculaDetalle/5 (por ejemplo)
    public async Task<IActionResult> PeliculaDetalle(int id)
    {
        // Buscamos la película específica por su ID
        var pelicula = await _context.Peliculas.FindAsync(id);

        if (pelicula == null)
        {
            // Si no se encuentra, mostramos un error 404
            return NotFound();
        }

        // Enviamos el objeto 'pelicula' a la nueva vista
        // !! NOTA: El nombre de la vista debe ser "PeliculaDetalle.cshtml"
        return View("PeliculaDetalle", pelicula);
    }

    // --- 3. ¡ACCIÓN AÑADIDA! ---
    // Esta es la acción que faltaba para los detalles de las series.
    // Responde a la URL: /Home/SerieDetalle/5 (por ejemplo)
    public async Task<IActionResult> SerieDetalle(int id)
    {
        // Buscamos la serie específica por su ID
        var serie = await _context.Series.FindAsync(id);

        if (serie == null)
        {
            // Si no se encuentra, mostramos un error 404
            return NotFound();
        }

        // Enviamos el objeto 'serie' a la nueva vista
        // !! NOTA: Debes crear una vista llamada "SerieDetalle.cshtml"
        return View("SerieDetalle", serie);
    }
}