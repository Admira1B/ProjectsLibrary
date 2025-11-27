using AutoMapper;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.DTOs.Company;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.MVC.Models.Company;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;

namespace ProjectsLibrary.MVC.ViewModelBuilders {
    public class CompanyViewModelBuilder(ICompanyService service, IMapper mapper) : ICompanyViewModelBuilder {
        private readonly ICompanyService _service = service;
        private readonly IMapper _mapper = mapper;

        public Task<IndexCompanyViewModel> BuildIndexViewModelAsync(CompanyAddDto? companyDto = null) {
            var model = new IndexCompanyViewModel {
                CompanyAddDto = companyDto
            };

            return Task.FromResult(model);
        }

        public async Task<DetailsCompanyViewModel?> BuildDetailsViewModelAsync(int id, CompanyUpdateDto? companyDto = null) {
            var company = await _service.GetCompanyWithProjectsNoTrackingAsync(id);
            if (company == null) {
                return null;
            }

            var model = new DetailsCompanyViewModel() {
                Id = id,
                Company = companyDto ?? _mapper.Map<CompanyUpdateDto>(company),
                Projects = _mapper.Map<List<ProjectReadDto>>(company.Projects)
            };

            return model;
        }
    }
}
