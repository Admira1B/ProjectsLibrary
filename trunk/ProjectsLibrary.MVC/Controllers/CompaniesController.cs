using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;

namespace ProjectsLibrary.MVC.Controllers {
    public class CompaniesController(ICompanyViewModelBuilder viewModelBuilder) : Controller {
        private readonly ICompanyViewModelBuilder _viewModelBuilder = viewModelBuilder;

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
    }
}
