using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Enums;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.MVC.Helpers;

namespace ProjectsLibrary.MVC.Controllers.Api {
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController(IProjectService service, IMapper mapper) : ControllerBase {
        private readonly IProjectService _service = service;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult> Get([FromQuery] GetPagedModel model) {
            var builtParams = ControllerHelper.BuildGetMethodModelParams(model);

            int? userId = null;
            var userRole = UserHelper.GetUserRole(User);

            if (userRole <= EmployeeRole.Manager) {
                userId = UserHelper.GetUserId(User);
            }

            var projectsPaged = await _service.GetPaginatedAsync(
                filterParams: builtParams.filterParams,
                sortParams: builtParams.sortParams,
                pageParams: builtParams.pageParams,
                employeeId: userId);

            var projectsDtos = _mapper.Map<List<ProjectReadDto>>(projectsPaged.Datas);

            var result = new PagedResult<ProjectReadDto>() {
                Datas = projectsDtos,
                FilteredRecords = projectsPaged.FilteredRecords,
                TotalRecords = projectsPaged.TotalRecords
            };

            return Ok(new {
                model.Draw,
                recordsFiltered = result.FilteredRecords,
                recordsTotal = result.TotalRecords,
                data = result.Datas
            });
        }

        [HttpGet("{id:int}")]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult> GetById(int id) {
            var project = await _service.GetByIdNoTrackingAsync(id);
            var projectDto = _mapper.Map<ProjectReadDto>(project);

            return Ok(projectDto);
        }

        [HttpGet("{id:int}/with-tasks")]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult<ProjectTasksInfoDto>> GetProjectWithTasks(int id) {
            var project = await _service.GetProjectWithTasksNoTrackingAsync(id);
            var projectDto = _mapper.Map<ProjectTasksInfoDto>(project);
            projectDto.Tasks = _mapper.Map<List<TaskReadDto>>(project.Tasks);

            return Ok(projectDto);
        }

        [HttpDelete("{id:int}")]  
        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        public async Task<ActionResult> Delete(int id) {
            await _service.DeleteAsync(id);

            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPatch("{projectId:int}/add/{employeeId:int}")] 
        public async Task<ActionResult> AddEmployeeToProject(int projectId, int employeeId) {
            await _service.AddEmployeeToProject(projectId, employeeId);

            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPatch("{projectId:int}/remove/{employeeId:int}")]
        public async Task<ActionResult> RemoveEmployeeFromProject(int projectId, int employeeId) {
            await _service.RemoveEmployeeFromProject(projectId, employeeId);

            return NoContent();
        }
    }
}
