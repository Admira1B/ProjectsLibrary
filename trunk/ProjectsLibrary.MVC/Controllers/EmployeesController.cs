using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.MVC.Models.Employee;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;

namespace ProjectsLibrary.MVC.Controllers {
    public class EmployeesController(IEmployeeViewModelBuilder viewModelBuilder, IEmployeeService service, IMapper mapper) : Controller {
        private readonly IEmployeeViewModelBuilder _viewModelBuilder = viewModelBuilder;
        private readonly IEmployeeService _service = service;
        private readonly IMapper _mapper = mapper;

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        public async Task<IActionResult> Index() {
            var model = await _viewModelBuilder.BuildIndexViewModelAsync(User);

            return View(model);
        }

        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<IActionResult> Add() {
            var model = await _viewModelBuilder.BuildAddViewModelAsync();

            return View(model);
        }

        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<IActionResult> Details(int id) {
            var model = await _viewModelBuilder.BuildDetailsViewModelAsync(id);

            if (model == null) {
                return NotFound();
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<ActionResult> Add(AddEmployeeViewModel model) {
            if (!ModelState.IsValid) {
                if (model == null || model.Employee == null) {
                    return View(model);
                }

                if (!string.IsNullOrEmpty(model.Employee.Password)) {
                    return View(model);
                }
            }

            var employee = _mapper.Map<Employee>(model.Employee);
            await _service.AddAsync(employee);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<ActionResult> Details(DetailsEmployeeViewModel model) {
            if (!ModelState.IsValid) {
                if (model == null || model.Employee == null) {
                    return View(model);
                }

                if (!string.IsNullOrEmpty(model.Employee.Password)) {
                    return View(model);
                }
            }

            var employee = _mapper.Map<Employee>(model.Employee);
            employee.Id = model.Id;
            await _service.UpdateAsync(employee);

            return RedirectToAction(nameof(Index));
        }
    }
}
