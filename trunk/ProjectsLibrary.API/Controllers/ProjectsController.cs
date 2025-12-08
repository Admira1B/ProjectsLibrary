using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.API.Helpers;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Project;

namespace ProjectsLibrary.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController(IProjectService service, IMapper mapper) : ControllerBase {
        private readonly IProjectService _service = service;
        private readonly IMapper _mapper = mapper;

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProjectReadDto>>> Get([FromQuery] GetPagedModel model) {
            var builtParams = ControllerHelper.BuildGetMethodModelParams(model);

            var projectsPaged = await _service.GetPaginatedAsync(
                filterParams: builtParams.filterParams,
                sortParams: builtParams.sortParams,
                pageParams: builtParams.pageParams);

            var projectsDtos = _mapper.Map<List<ProjectReadDto>>(projectsPaged.Datas);

            var result = new PagedResult<ProjectReadDto>() {
                Datas = projectsDtos,
                FilteredRecords = projectsPaged.FilteredRecords,
                TotalRecords = projectsPaged.TotalRecords
            };

            return Ok(result);
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProjectReadDto>> GetById([FromRoute] int id) {
            var project = await _service.GetByIdNoTrackingAsync(id);
            var projectDto = _mapper.Map<ProjectReadDto>(project);
            return Ok(projectDto);
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] ProjectAddDto projectDto) {
            var project = _mapper.Map<Project>(projectDto);
            await _service.AddAsync(project);
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, _mapper.Map<ProjectReadDto>(project));
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] ProjectUpdateDto projectDto) {
            var project = _mapper.Map<Project>(projectDto);
            project.Id = id;
            await _service.UpdateAsync(project);
            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id) {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPatch("{projectId:int}/add/{employeeId:int}")]
        public async Task<ActionResult> AddEmployeeToProject([FromRoute] int projectId, [FromRoute] int employeeId) {
            await _service.AddEmployeeToProject(projectId, employeeId);
            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPatch("{projectId:int}/remove/{employeeId:int}")]
        public async Task<ActionResult> RemoveEmployeeFromProject([FromRoute] int projectId, [FromRoute] int employeeId) {
            await _service.RemoveEmployeeFromProject(projectId, employeeId);
            return NoContent();
        }
    }
}
