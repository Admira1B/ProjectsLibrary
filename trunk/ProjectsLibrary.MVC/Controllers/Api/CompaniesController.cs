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

namespace ProjectsLibrary.MVC.Controllers.Api {
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController(ICompanyService service, IMapper mapper) : ControllerBase {
        private readonly ICompanyService _service = service;
        private readonly IMapper _mapper = mapper;

        [HttpGet]
        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<ActionResult> Get([FromQuery]GetPagedModel model) {
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

            return Ok (new {
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

            return Ok(new { success = true });
        }

        [HttpPut("{id:int}")]
        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        public async Task<ActionResult> Update(int id, CompanyUpdateDto company) {
            var companyEntity = _mapper.Map<Company>(company);
            companyEntity.Id = id;
            await _service.UpdateAsync(companyEntity);

            return Ok(new { success = true });
        }

        [Authorize(Policy = PolicyLevelName.SupervisorLevel)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id) {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
