using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.MVC.Models;
using ProjectsLibrary.MVC.Models.Home;

namespace ProjectsLibrary.MVC.Controllers
{
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

        public IActionResult Registration(string email) 
        {
            var model = new RegistrationViewModel { InitialEmail = email };
            return View(model);
        }

        public IActionResult Login() 
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
