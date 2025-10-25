using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PROYECTOMOVIE.Models;

namespace PROYECTOMOVIE.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
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
}


