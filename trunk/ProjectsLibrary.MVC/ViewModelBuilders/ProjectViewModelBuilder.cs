using AutoMapper;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.DTOs.Company;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.MVC.Helpers;
using ProjectsLibrary.MVC.Models.Project;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;
using System.Security.Claims;

namespace ProjectsLibrary.MVC.ViewModelBuilders {
    public class ProjectViewModelBuilder(IProjectService service, IEmployeeService employeeService, ICompanyService companyService, IMapper mapper) : IProjectViewModelBuilder {
        private readonly IProjectService _service = service;
        private readonly IEmployeeService _employeeService = employeeService;
        private readonly ICompanyService _companyService = companyService;
        private readonly IMapper _mapper = mapper;

        public Task<IndexProjectViewModel> BuildIndexViewModelAsync(ClaimsPrincipal user) {
            var model = new IndexProjectViewModel {
                UserRole = UserHelper.GetUserRole(user)
            };

            return Task.FromResult(model);
        }

        public async Task<AddProjectViewModel> BuildAddViewModelAsync(ProjectAddDto? projectDto = null) {
            var employees = await _employeeService.GetDataOnlyAsync();
            var companies = await _companyService.GetDataOnlyAsync();

            var model = new AddProjectViewModel() {
                Project = projectDto,
                Employees = _mapper.Map<List<EmployeeReadDto>>(employees),
                Companies = _mapper.Map<List<CompanyReadDto>>(companies),
            };

            return model;
        }

        public async Task<DetailsProjectViewModel?> BuildDetailsViewModelAsync(int id, ProjectUpdateDto? projectDto = null) {
            var project = await _service.GetByIdAsync(id);
            if (project == null) {
                return null;
            }

            var employees = await _employeeService.GetDataOnlyAsync();
            var companies = await _companyService.GetDataOnlyAsync();
            var projectManager = await _employeeService.GetByIdAsync(project.ProjectManagerId);

            var model = new DetailsProjectViewModel() {
                Id = id,
                Project = projectDto ?? _mapper.Map<ProjectUpdateDto>(project),
                ProjectManager = _mapper.Map<EmployeeShortDto>(projectManager),
                Employees = _mapper.Map<List<EmployeeReadDto>>(employees),
                Companies = _mapper.Map<List<CompanyReadDto>>(companies)
            };

            return model;
        }
    }
}
