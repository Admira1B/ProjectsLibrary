using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.MVC.Models.Project;
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

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        public async Task<IActionResult> Add(AddProjectViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }

            var project = _mapper.Map<Project>(model.Project);
            await _service.AddAsync(project);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        public async Task<IActionResult> Details(DetailsProjectViewModel model) {
            if (!ModelState.IsValid) {
                if (!(model.ProjectManager == null)) {
                    return View(model);
                }
            }

            var project = _mapper.Map<Project>(model.Project);
            project.Id = model.Id;
            await _service.UpdateAsync(project);

            return RedirectToAction(nameof(Index));
        }
    }
}
