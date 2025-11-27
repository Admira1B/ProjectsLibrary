using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.MVC.Helpers;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;

namespace ProjectsLibrary.MVC.Controllers {
    public class EmployeesController(IEmployeeViewModelBuilder viewModelBuilder, IEmployeeService service, IProjectService projectService, IMapper mapper) : Controller {
        private readonly IEmployeeViewModelBuilder _viewModelBuilder = viewModelBuilder;
        private readonly IEmployeeService _service = service;
        private readonly IProjectService _projectService = projectService;
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

        [HttpGet]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult> GetDataOnlyWithoutWorking([FromRoute] int id) {
            var project = await _projectService.GetByIdNoTrackingAsync(id);
            var employees = await _service.GetDataOnlyAsync();

            var projectEmployeeIds = new HashSet<int>(project.Employees.Select(e => e.Id))
            {
                project.ProjectManagerId
            };

            var result = employees.Where(emp => !projectEmployeeIds.Contains(emp.Id)).ToList();
            var resultMapped = _mapper.Map<List<EmployeeReadDto>>(result);

            return Ok(resultMapped);
        }

        [HttpGet]
        public async Task<ActionResult> GetEmployeeWithTasks([FromRoute] int id) {
            var employee = await _service.GetEmployeeWithTasksNoTrackingAsync(id);
            var employeeDto = _mapper.Map<EmployeeReadDto>(employee);

            return Ok(employeeDto);
        }

        [HttpGet]
        public async Task<ActionResult> GetEmployeeWithProjects([FromRoute] int id) {
            var employee = await _service.GetEmployeeWithProjectsNoTrackingAsync(id);
            var employeeDto = _mapper.Map<EmployeeReadDto>(employee);

            return Ok(employeeDto);
        }

        [HttpPost]
        public async Task<ActionResult> Get(GetPagedModel model) {
            var builtParams = ControllerHelper.BuildGetMethodModelParams(model);

            var employeesPaged = await _service.GetPaginatedAsync(
                filterParams: builtParams.filterParams,
                sortParams: builtParams.sortParams,
                pageParams: builtParams.pageParams);

            var employeesDtos = _mapper.Map<List<EmployeeReadDto>>(employeesPaged.Datas);

            var result = new PagedResult<EmployeeReadDto>() {
                Datas = employeesDtos,
                FilteredRecords = employeesPaged.FilteredRecords,
                TotalRecords = employeesPaged.TotalRecords
            };

            return Json
            (new {
                model.Draw,
                recordsFiltered = result.FilteredRecords,
                recordsTotal = result.TotalRecords,
                data = result.Datas
            });
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<ActionResult> Add(EmployeeAddDto employee) {
            if (!ModelState.IsValid) {
                var model = _viewModelBuilder.BuildAddViewModelAsync(employee);

                return View("Add", model);
            }

            var employeeEntity = _mapper.Map<Employee>(employee);
            await _service.AddAsync(employeeEntity);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<ActionResult> Update([FromRoute] int id, EmployeeUpdateDto employee) {
            if (!ModelState.IsValid) {
                var model = _viewModelBuilder.BuildDetailsViewModelAsync(id, employee);

                if (model == null) {
                    return NotFound();
                }

                return View("Details", model);
            }

            var employeeEntity = _mapper.Map<Employee>(employee);
            employeeEntity.Id = id;
            await _service.UpdateAsync(employeeEntity);

            return RedirectToAction("Index");
        }

        [HttpDelete]
        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<ActionResult> Delete([FromRoute] int id) {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPatch("Employees/{employeeId:int}/assign/{taskId:int}")]
        public async Task<ActionResult> AssignTaskToEmployee([FromRoute] int employeeId, [FromRoute] int taskId) {
            await _service.AssignTaskToEmployee(employeeId, taskId);
            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPatch("Employees/{employeeId:int}/unassign/{taskId:int}")]
        public async Task<ActionResult> UnassignTaskFromEmployee([FromRoute] int employeeId, [FromRoute] int taskId) {
            await _service.UnassignTaskToEmployee(employeeId, taskId);
            return NoContent();
        }
    }
}
