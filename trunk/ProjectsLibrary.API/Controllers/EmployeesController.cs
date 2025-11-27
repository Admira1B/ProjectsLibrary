using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.API.Extensions;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Employee;

namespace ProjectsLibrary.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController(IEmployeeService service, IMapper mapper) : ControllerBase {
        private readonly IEmployeeService _service = service;
        private readonly IMapper _mapper = mapper;

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] EmployeeAddDto employeeDto) {
            var employee = _mapper.Map<Employee>(employeeDto);

            await _service.RegisterAsync(employee, employeeDto.Password);

            return NoContent();
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(EmployeeLoginDto loginDto) {
            var token = await _service.LoginAsync(loginDto.Email, loginDto.Password);

            AppendTokenToCookies(token);

            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        [HttpGet]
        public async Task<ActionResult<PagedResult<EmployeeReadDto>>> Get([FromQuery] GetPagedModel model) {
            var builtParams = ControllersExtensions.BuildGetMethodModelParams(model);

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

            return Ok(result);
        }

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeReadDto>> GetById([FromRoute] int id) {
            var employee = await _service.GetByIdNoTrackingAsync(id);
            var employeeDto = _mapper.Map<EmployeeReadDto>(employee);
            return Ok(employeeDto);
        }

        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] EmployeeAddDto employeeDto) {
            var employee = _mapper.Map<Employee>(employeeDto);
            await _service.AddAsync(employee);
            return CreatedAtAction(nameof(GetById), new { id = employee.Id }, _mapper.Map<EmployeeReadDto>(employee));
        }

        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] EmployeeUpdateDto employeeDto) {
            var employee = _mapper.Map<Employee>(employeeDto);
            employee.Id = id;
            await _service.UpdateAsync(employee);
            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id) {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPatch("{employeeId:int}/assign/{taskId:int}")]
        public async Task<ActionResult> AssignTaskToEmployee([FromRoute] int employeeId, [FromRoute] int taskId) {
            await _service.AssignTaskToEmployee(employeeId, taskId);
            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPatch("{employeeId:int}/unassign/{taskId:int}")]
        public async Task<ActionResult> UnassignTaskFromEmployee([FromRoute] int employeeId, [FromRoute] int taskId) {
            await _service.UnassignTaskToEmployee(employeeId, taskId);
            return NoContent();
        }

        private void AppendTokenToCookies(string token) {
            HttpContext.Response.Cookies.Append("auth-t", token);
        }
    }
}
