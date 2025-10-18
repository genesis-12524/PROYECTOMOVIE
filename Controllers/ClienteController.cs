using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace PROYECTOMOVIE.Controllers
{
    [Authorize(Roles = "Cliente")]
    public class ClienteController : Controller
    {

        private readonly ILogger<ClienteController> _logger;


        public ClienteController(ILogger<ClienteController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}