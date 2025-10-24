using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.API.Extencions;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Company;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.MVC.Models.Company;

namespace ProjectsLibrary.MVC.Controllers
{
    public class CompaniesController(ICompanyService service, IMapper mapper) : Controller
    {
        private readonly ICompanyService _service = service;
        private readonly IMapper _mapper = mapper;

        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public IActionResult Index()
        {
            var model = new IndexCompanyViewModel();

            return View(model);
        }
        
        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<IActionResult> Details(int id)
        {
            var company = await _service.GetCompanyWithProjectsNoTrackingAsync(id);
            if (company == null)
            {
                return NotFound();
            }

            var comapnyUpdateDto = _mapper.Map<CompanyUpdateDto>(company);
            var projectsReadDtos = _mapper.Map<List<ProjectReadDto>>(company.Projects);

            var model = new DetailsCompanyViewModel()
            {
                Id = id,
                Company = comapnyUpdateDto,
                Projects = projectsReadDtos
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<ActionResult> Get(GetPagedModel model) 
        {
            var builtParams = ControllersExtencions.BuildGetMethodModelParams(model);

            var companiesPaged = await _service.GetPaginatedAsync(
                filterParams: builtParams.filterParams,
                sortParams: builtParams.sortParams,
                pageParams: builtParams.pageParams);

            var companiesDtos = _mapper.Map<List<CompanyReadDto>>(companiesPaged.Datas);

            var result = new PagedResult<CompanyReadDto>()
            {
                Datas = companiesDtos,
                FilteredRecords = companiesPaged.FilteredRecords,
                TotalRecords = companiesPaged.TotalRecords
            };

            return Json
            (new
            {
                model.Draw,
                recordsFiltered = result.FilteredRecords,
                recordsTotal = result.TotalRecords,
                data = result.Datas
            });
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<ActionResult> Add(CompanyAddDto companyAddDto)
        {
            var company = _mapper.Map<Company>(companyAddDto);
            await _service.AddAsync(company);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<IActionResult> Update([FromRoute] int id, CompanyUpdateDto company)
        {
            var companyEntity = _mapper.Map<Company>(company);
            companyEntity.Id = id;
            await _service.UpdateAsync(companyEntity);
         
            return RedirectToAction("Index");
        }

        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        [HttpDelete("Company/Delete/{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        private async Task<CompanyReadDto> GetById(int id)
        {
            var company = await _service.GetByIdNoTrackingAsync(id);
            var companyDto = _mapper.Map<CompanyReadDto>(company);
            return companyDto;
        }
    }
}
