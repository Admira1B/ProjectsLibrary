using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.Enums;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.MVC.Helpers;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;

namespace ProjectsLibrary.MVC.Controllers {
    public class TasksController(ITaskViewModelBuilder viewModelBuilder, ITaskService service, IMapper mapper) : Controller {
        private readonly ITaskViewModelBuilder _viewModelBuilder = viewModelBuilder;
        private readonly ITaskService _service = service;
        private readonly IMapper _mapper = mapper;

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<IActionResult> Index() {
            var model = await _viewModelBuilder.BuildIndexViewModelAsync(User);

            return View(model);
        }

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<IActionResult> Add(int? id = null) {
            var model = await _viewModelBuilder.BuildAddViewModelAsync(User, id);

            return View(model);
        }

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<IActionResult> Details(int id) {
            var model = await _viewModelBuilder.BuildDetailsViewModelAsync(id, User);

            if (model == null) {
                return NotFound();
            }

            return View(model);
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

            var tasksPaged = await _service.GetPaginatedAsync(
                filterParams: builtParams.filterParams,
                sortParams: builtParams.sortParams,
                pageParams: builtParams.pageParams,
                employeeId: userId);

            var tasksDtos = _mapper.Map<List<TaskReadDto>>(tasksPaged.Datas);

            var result = new PagedResult<TaskReadDto>() {
                Datas = tasksDtos,
                FilteredRecords = tasksPaged.FilteredRecords,
                TotalRecords = tasksPaged.TotalRecords
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
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult> Add(TaskAddDto task) {
            if (!ModelState.IsValid) {
                var model = await _viewModelBuilder.BuildAddViewModelAsync(User, null, task);
                return View("Add", model);
            }

            var taskEntity = _mapper.Map<TaskPL>(task);
            await _service.AddAsync(taskEntity);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult> Update([FromRoute] int id, TaskUpdateDto task) {
            if (!ModelState.IsValid) {
                var model = await _viewModelBuilder.BuildDetailsViewModelAsync(id, User, task);

                if (model == null) {
                    return NotFound();
                }

                return View("Details", model);
            }

            var taskEntity = _mapper.Map<TaskPL>(task);
            taskEntity.Id = id;
            await _service.UpdateAsync(taskEntity);

            return RedirectToAction("Index");
        }

        [HttpDelete]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult> Delete([FromRoute] int id) {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
