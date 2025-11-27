using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.Enums;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.MVC.Helpers;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;

namespace ProjectsLibrary.MVC.Controllers {
    public class ProjectsController(IProjectViewModelBuilder viewModelBuilder, IProjectService service, IMapper mapper) : Controller {
        private readonly IProjectViewModelBuilder _viewModelBuilder = viewModelBuilder;
        private readonly IProjectService _service = service;
        private readonly IMapper _mapper = mapper;

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<IActionResult> Index() {
            var model = await _viewModelBuilder.BuildIndexViewModelAsync(User);

            return View(model);
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        public async Task<IActionResult> Add() {
            var model = await _viewModelBuilder.BuildAddViewModelAsync();

            return View(model);
        }

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<IActionResult> Details(int id) {
            var model = await _viewModelBuilder.BuildDetailsViewModelAsync(id);

            if (model == null) {
                return NotFound();
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ProjectReadDto> GetById(int id) {
            var project = await _service.GetByIdNoTrackingAsync(id);
            var projectDto = _mapper.Map<ProjectReadDto>(project);
            return projectDto;
        }

        [HttpGet]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult<ProjectTasksInfoDto>> GetProjectWithTasks(int id) {
            var project = await _service.GetProjectWithTasksNoTrackingAsync(id);
            var projectDto = _mapper.Map<ProjectTasksInfoDto>(project);
            projectDto.Tasks = _mapper.Map<List<TaskReadDto>>(project.Tasks);

            return Ok(projectDto);
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult> Get(GetPagedModel model) {
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

            return Json
            (new {
                model.Draw,
                recordsFiltered = result.FilteredRecords,
                recordsTotal = result.TotalRecords,
                data = result.Datas
            });
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        public async Task<IActionResult> Add(ProjectAddDto project) {
            if (!ModelState.IsValid) {
                var model = await _viewModelBuilder.BuildAddViewModelAsync(project);
                return View("Add", model);
            }

            var projectEntity = _mapper.Map<Project>(project);
            await _service.AddAsync(projectEntity);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        public async Task<IActionResult> Update([FromRoute] int id, ProjectUpdateDto project) {
            if (!ModelState.IsValid) {
                var model = await _viewModelBuilder.BuildDetailsViewModelAsync(id, project);

                if (model == null) {
                    return NotFound();
                }

                return View("Details", model);
            }

            var projectEntity = _mapper.Map<Project>(project);
            projectEntity.Id = id;
            await _service.UpdateAsync(projectEntity);

            return RedirectToAction("Index");
        }

        [HttpDelete]
        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        public async Task<ActionResult> Delete([FromRoute] int id) {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPatch("Projects/{projectId:int}/add/{employeeId:int}")]
        public async Task<ActionResult> AddEmployeeToProject([FromRoute] int projectId, [FromRoute] int employeeId) {
            await _service.AddEmployeeToProject(projectId, employeeId);
            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPatch("Projects/{projectId:int}/remove/{employeeId:int}")]
        public async Task<ActionResult> RemoveEmployeeFromProject([FromRoute] int projectId, [FromRoute] int employeeId) {
            await _service.RemoveEmployeeFromProject(projectId, employeeId);
            return NoContent();
        }
    }
}
