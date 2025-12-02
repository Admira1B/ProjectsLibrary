using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.MVC.Models.Task;
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
        public async Task<ActionResult> Add(AddTaskViewModel model) {
            if (!ModelState.IsValid) {
                    return View(model);
            }

            var task = _mapper.Map<TaskPL>(model.Task);
            await _service.AddAsync(task);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult> Details(DetailsTaskViewModel model) {
            if (!ModelState.IsValid) {
                if (!(model.TaskCreator == null)) {
                    return View(model);
                }
            }

            var task = _mapper.Map<TaskPL>(model.Task);
            task.Id = model.Id;
            await _service.UpdateAsync(task);

            return RedirectToAction(nameof(Index));
        }
    }
}
