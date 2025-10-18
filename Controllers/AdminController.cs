using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PROYECTOMOVIE.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {

        private readonly ILogger<AdminController> _logger;


        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }        
    }
}