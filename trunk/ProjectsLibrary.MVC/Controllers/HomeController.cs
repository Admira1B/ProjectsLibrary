using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Exceptions;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.MVC.Models;
using ProjectsLibrary.MVC.Models.Home;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;
using System.Diagnostics;

namespace ProjectsLibrary.MVC.Controllers {
    public class HomeController(IHomeViewModelBuilder viewModelBuilder, IMapper mapper, IEmployeeService employeeService) : Controller {
        private readonly IHomeViewModelBuilder _viewModelBuilder = viewModelBuilder;
        private readonly IMapper _mapper = mapper;
        private readonly IEmployeeService _employeeService = employeeService;

        public IActionResult Index() {
            return View();
        }

        public async Task<IActionResult> Registration(string email) {
            ModelState.Clear();

            var model = await _viewModelBuilder.BuildRegistrationViewModelAsync(email);

            return View(model);
        }

        public async Task<IActionResult> Login() {
            var model = await _viewModelBuilder.BuildLoginViewModelAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout() {
            Response.Cookies.Delete("auth-t");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<ActionResult> Register(EmployeeAddDto employee) {
            try {
                var employeeEntity = _mapper.Map<Employee>(employee);

                await _employeeService.RegisterAsync(employeeEntity, employee.Password);

                return RedirectToAction("Login");
            } catch (EmployeeAlreadyExistsException ex) {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            var model = new RegistrationViewModel {
                Employee = employee,
            };

            return View("Registration", model);
        }

        [HttpPost]
        public async Task<ActionResult> Login(EmployeeLoginDto employee) {
            try {
                var token = await _employeeService.LoginAsync(employee.Email, employee.Password);

                AppendTokenToCookies(token);

                return RedirectToAction("Index", "Tasks");
            } catch (EmailNotRegisteredException ex) {
                ModelState.AddModelError(string.Empty, ex.Message);
            } catch (CreatedEmployeeNotRegisteredException ex) {
                ModelState.AddModelError(string.Empty, ex.Message);
            } catch (IncorrectEmployeePasswordException ex) {
                ModelState.AddModelError(string.Empty, ex.Message);
            }

            var model = new LoginViewModel {
                Employee = employee,
            };

            return View("Login", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void AppendTokenToCookies(string token) {
            var cookieOptions = new CookieOptions {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Strict,
            };

            Response.Cookies.Append("auth-t", token, cookieOptions);
        }
    }
}
