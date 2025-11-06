using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PROYECTOMOVIE.Models;
using Microsoft.EntityFrameworkCore;
using PROYECTOMOVIE.Data;

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
        return View();
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

    // --- 2. AQUÍ ESTÁ LA NUEVA ACCIÓN DE BÚSQUEDA ---
    public async Task<IActionResult> Buscar(string terminoBusqueda)
    {
        // 3. Prepara la consulta
        var peliculasQuery = from peli in _context.Peliculas
                             select peli;

        // 4. Si el término no está vacío, filtra la consulta
        if (!string.IsNullOrEmpty(terminoBusqueda))
        {

            // para que la comparación sea insensible a mayúsculas/minúsculas.
            string terminoEnMinusculas = terminoBusqueda.ToLower();

            peliculasQuery = peliculasQuery.Where(
                p => p.Nombre_Peli != null && 
                    p.Nombre_Peli.ToLower().Contains(terminoEnMinusculas)
        );
        }

        // 5. Ejecuta la consulta y envía los resultados a una NUEVA vista
        var peliculas = await peliculasQuery.ToListAsync();

        // Pasamos el término de búsqueda a la vista, para poder mostrarlo
        ViewData["TerminoBusqueda"] = terminoBusqueda;

        return View("ResultadosBusqueda", peliculas);
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
        // (Nota: Skip() puede ser lento en bases de datos gigantes,
        // pero es perfecto para la mayoría de proyectos)
        var randomPelicula = await _context.Peliculas
                                           .Skip(index)
                                           .FirstOrDefaultAsync();

        if (randomPelicula == null)
        {
            return RedirectToAction("Index");
        }

        // 4. Redirigimos al usuario a la página de Detalles de esa película
        // (Asumiendo que tu modelo 'Pelicula' tiene una propiedad 'Id')
        return RedirectToAction("PeliculaAleDetalle", new { id = randomPelicula.Id }); 
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
        return View("PeliculaAleDetalle", pelicula);
    }
}


