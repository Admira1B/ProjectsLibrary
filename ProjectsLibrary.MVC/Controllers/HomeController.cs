using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Exceptions;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.MVC.Models;
using ProjectsLibrary.MVC.Models.Home;
using System.Diagnostics;

namespace ProjectsLibrary.MVC.Controllers
{
    public class HomeController(ILogger<HomeController> logger, IMapper mapper, IEmployeeService employeeService) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IEmployeeService _employeeService = employeeService;

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Registration(string email) 
        {
            ModelState.Clear();
            var model = new RegistrationViewModel { InitialEmail = email };
            return View(model);
        }

        public IActionResult Login() 
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("auth-t");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> Register(EmployeeAddDto employee)
        {
            try
            {
                var employeeEntity = _mapper.Map<Employee>(employee);

                await _employeeService.RegisterAsync(employeeEntity, employee.Password);

                return RedirectToAction(actionName: "Login", controllerName: "Home");
            }
            catch (EmployeeAlreadyExistsException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            var model = new RegistrationViewModel
            {
                Employee = employee,
            };

            return View("Registration", model);
        }

        [HttpPost]
        public async Task<ActionResult> Login(EmployeeLoginDto employee)
        {
            try
            {
                var token = await _employeeService.LoginAsync(employee.Email, employee.Password);

                AppendTokenToCookies(token);

                return RedirectToAction("Index", "Tasks");
            }
            catch (EmailNotRegisteredException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (CreatedEmployeeNotRegisteredException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (IncorrectEmployeePasswordException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            var model = new LoginViewModel 
            {
                Employee = employee,
            };

            return View("Login", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void AppendTokenToCookies(string token)
        {
            HttpContext.Response.Cookies.Append("auth-t", token);
        }
    }
}
