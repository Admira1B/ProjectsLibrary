using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProjectsLibrary.MVC.Helpers;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.CompositionRoot.Autorization;

namespace ProjectsLibrary.MVC.Controllers.Api {
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController(IEmployeeService service, IProjectService projectService, IMapper mapper) : ControllerBase {
        private readonly IEmployeeService _service = service;
        private readonly IProjectService _projectService = projectService;
        private readonly IMapper _mapper = mapper;

        [HttpGet("{id:int}/available-employees")]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult> GetAvailable(int id) {
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

        [HttpGet("{id:int}/with-tasks")]
        public async Task<ActionResult> GetEmployeeWithTasks(int id) {
            var employee = await _service.GetEmployeeWithTasksNoTrackingAsync(id);
            var employeeDto = _mapper.Map<EmployeeReadDto>(employee);

            return Ok(employeeDto);
        }

        [HttpGet("{id:int}/with-projects")]
        public async Task<ActionResult> GetEmployeeWithProjects(int id) {
            var employee = await _service.GetEmployeeWithProjectsNoTrackingAsync(id);
            var employeeDto = _mapper.Map<EmployeeReadDto>(employee);

            return Ok(employeeDto);
        }

        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] GetPagedModel model) {
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

            return Ok(new {
                model.Draw,
                recordsFiltered = result.FilteredRecords,
                recordsTotal = result.TotalRecords,
                data = result.Datas
            });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<ActionResult> Delete(int id) {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPatch("{employeeId:int}/assign/{taskId:int}")]
        public async Task<ActionResult> AssignTaskToEmployee(int employeeId, int taskId) {
            await _service.AssignTaskToEmployee(employeeId, taskId);
            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPatch("{employeeId:int}/unassign/{taskId:int}")]
        public async Task<ActionResult> UnassignTaskFromEmployee(int employeeId, int taskId) {
            await _service.UnassignTaskToEmployee(employeeId, taskId);
            return NoContent();
        }
    }
}
