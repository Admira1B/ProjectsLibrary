using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.MVC.Models.Company;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;

namespace ProjectsLibrary.MVC.Controllers {
    public class CompaniesController(ICompanyViewModelBuilder viewModelBuilder, ICompanyService service, IMapper mapper) : Controller {
        private readonly ICompanyViewModelBuilder _viewModelBuilder = viewModelBuilder;
        private readonly ICompanyService _service = service;
        private readonly IMapper _mapper = mapper;

        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<IActionResult> Index() {
            var model = await _viewModelBuilder.BuildIndexViewModelAsync();

            return View(model);
        }

        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<IActionResult> Details(int id) {
            var model = await _viewModelBuilder.BuildDetailsViewModelAsync(id);

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<IActionResult> Details(DetailsCompanyViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }

            var company = _mapper.Map<Company>(model.Company);
            company.Id = model.Id;
            await _service.UpdateAsync(company);

            return RedirectToAction(nameof(Index));
        }
    }
}
