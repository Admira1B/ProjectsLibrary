using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.API.Extensions;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Company;

namespace ProjectsLibrary.API.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class CompaniesController(ICompanyService service, IMapper mapper) : ControllerBase {
        private readonly ICompanyService _service = service;
        private readonly IMapper _mapper = mapper;

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        [HttpGet]
        public async Task<ActionResult<PagedResult<CompanyReadDto>>> Get([FromQuery] GetPagedModel model) {
            var builtParams = ControllersExtensions.BuildGetMethodModelParams(model);

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

            return Ok(result);
        }

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CompanyReadDto>> GetById([FromRoute] int id) {
            var company = await _service.GetByIdNoTrackingAsync(id);
            var companyDto = _mapper.Map<CompanyReadDto>(company);
            return Ok(companyDto);
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPost]
        public async Task<ActionResult> Add([FromBody] CompanyAddDto companyDto) {
            var company = _mapper.Map<Company>(companyDto);
            await _service.AddAsync(company);
            return CreatedAtAction(nameof(GetById), new { id = company.Id }, _mapper.Map<CompanyReadDto>(company));
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update([FromRoute] int id, [FromBody] CompanyUpdateDto companyDto) {
            var company = _mapper.Map<Company>(companyDto);
            company.Id = id;
            await _service.UpdateAsync(company);
            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id) {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
