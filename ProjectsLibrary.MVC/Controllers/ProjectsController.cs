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
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.MVC.Models.Project;

namespace ProjectsLibrary.MVC.Controllers
{
    public class ProjectsController(IProjectService service, IEmployeeService employeeService, ICompanyService companyService, IMapper mapper) : Controller
    {
        private readonly IProjectService _service = service;
        private readonly IEmployeeService _employeeService = employeeService;
        private readonly ICompanyService _companyService = companyService;
        private readonly IMapper _mapper = mapper;

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public IActionResult Index()
        {
            var model = new IndexProjectViewModel
            {
                UserRole = ControllersExtencions.GetUserRole(User)
            };

            return View(model);
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        public async Task<IActionResult> Add()
        {
            var employees = await _employeeService.GetDataOnlyAsync();
            var companies = await _companyService.GetDataOnlyAsync();

            var employeesDtos = _mapper.Map<List<EmployeeReadDto>>(employees);
            var companiesDtos = _mapper.Map<List<CompanyReadDto>>(companies);

            var model = new AddProjectViewModel() 
            {
                Employees = employeesDtos,
                Companies = companiesDtos,
            };

            return View(model);
        }

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<IActionResult> Details(int id)
        {
            var project = await _service.GetByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var projectDto = _mapper.Map<ProjectUpdateDto>(project);

            var employees = await _employeeService.GetDataOnlyAsync();
            var companies = await _companyService.GetDataOnlyAsync();

            var employeesDtos = _mapper.Map<List<EmployeeReadDto>>(employees);
            var companiesDtos = _mapper.Map<List<CompanyReadDto>>(companies);

            var model = new DetailsProjectViewModel()
            {
                Id = id,
                Project = projectDto,
                Employees = employeesDtos,
                Companies = companiesDtos,
            };

            return View(model);
        }

        [HttpGet]
        public async Task<ProjectReadDto> GetById(int id)
        {
            var project = await _service.GetByIdNoTrackingAsync(id);
            var projectDto = _mapper.Map<ProjectReadDto>(project);
            return projectDto;
        }

        [HttpGet]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult<ProjectTasksInfoDto>> GetProjectWithTasks(int id)
        {
            var project = await _service.GetProjectWithTasksNoTrackingAsync(id);
            var projectDto = _mapper.Map<ProjectTasksInfoDto>(project);
            projectDto.Tasks = _mapper.Map<List<TaskReadDto>>(project.Tasks);

            return Ok(projectDto);
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult<PagedResult<ProjectReadDto>>> Get(GetPagedModel model)
        {
            var builtParams = ControllersExtencions.BuildGetMethodModelParams(model);

            var projectsPaged = await _service.GetPaginatedAsync(
                filterParams: builtParams.filterParams,
                sortParams: builtParams.sortParams,
                pageParams: builtParams.pageParams);

            var projectsDtos = _mapper.Map<List<ProjectReadDto>>(projectsPaged.Datas);

            var result = new PagedResult<ProjectReadDto>()
            {
                Datas = projectsDtos,
                FilteredRecords = projectsPaged.FilteredRecords,
                TotalRecords = projectsPaged.TotalRecords
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
        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        public async Task<IActionResult> Add(ProjectAddDto project)
        {
            var projectEntity = _mapper.Map<Project>(project);
            await _service.AddAsync(projectEntity);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        public async Task<IActionResult> Update([FromRoute] int id, ProjectUpdateDto project)
        {
            var projectEntity = _mapper.Map<Project>(project);
            projectEntity.Id = id;
            await _service.UpdateAsync(projectEntity);

            return RedirectToAction("Index");
        }

        [HttpDelete]
        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPatch("Projects/{projectId:int}/add/{employeeId:int}")]
        public async Task<ActionResult> AddEmployeeToProject([FromRoute] int projectId, [FromRoute] int employeeId)
        {
            await _service.AddEmployeeToProject(projectId, employeeId);
            return NoContent();
        }

        [Authorize(Policy = PolicyLevelName.ManagmentLevel)]
        [HttpPatch("Projects/{projectId:int}/remove/{employeeId:int}")]
        public async Task<ActionResult> RemoveEmployeeFromProject([FromRoute] int projectId, [FromRoute] int employeeId)
        {
            await _service.RemoveEmployeeFromProject(projectId, employeeId);
            return NoContent();
        }
    }
}
