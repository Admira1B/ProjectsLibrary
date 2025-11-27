using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Company;
using ProjectsLibrary.MVC.Helpers;
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
        public async Task<ActionResult> Get(GetPagedModel model) {
            var builtParams = ControllerHelper.BuildGetMethodModelParams(model);

            var companiesPaged = await _service.GetPaginatedAsync(
                filterParams: builtParams.filterParams,
                sortParams: builtParams.sortParams,
                pageParams: builtParams.pageParams);

            var companiesDtos = _mapper.Map<List<CompanyReadDto>>(companiesPaged.Datas);

            var result = new PagedResult<CompanyReadDto>() {
                Datas = companiesDtos,
                FilteredRecords = companiesPaged.FilteredRecords,
                TotalRecords = companiesPaged.TotalRecords
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
        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<ActionResult> Add(CompanyAddDto companyAddDto) {
            var company = _mapper.Map<Company>(companyAddDto);
            await _service.AddAsync(company);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<IActionResult> Update([FromRoute] int id, CompanyUpdateDto company) {
            var companyEntity = _mapper.Map<Company>(company);
            companyEntity.Id = id;
            await _service.UpdateAsync(companyEntity);

            return RedirectToAction("Index");
        }

        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        [HttpDelete("Companies/Delete/{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id) {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
