using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.API.Extencions;
using ProjectsLibrary.CompositionRoot.Autorization;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.Enums;
using ProjectsLibrary.Domain.Models.RequestModels;
using ProjectsLibrary.Domain.Models.Results;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.MVC.Models.Task;

namespace ProjectsLibrary.MVC.Controllers
{
    public class TasksController(ITaskService service, IProjectService projectService, IEmployeeService employeeService, IMapper mapper) : Controller
    {
        private readonly ITaskService _service = service;
        private readonly IProjectService _projectService = projectService;
        private readonly IEmployeeService _employeeService = employeeService;
        private readonly IMapper _mapper = mapper;

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public IActionResult Index()
        {
            var model = new IndexTaskViewModel 
            {
                UserRole = ControllersExtencions.GetUserRole(User)
            };

            return View(model);
        }

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<IActionResult> Add(int? id = null)
        {
            List<Project> projects;
            int userId;
            var userRole = ControllersExtencions.GetUserRole(User);

            if (userRole <= EmployeeRole.Manager)
            {
                userId = ControllersExtencions.GetUserId(User);
                projects = await _employeeService.GetEmployeeAllProjectsByIdNoTrackingAsync(userId);
            }
            else
            {
                projects = await _projectService.GetDataOnlyAsync();
            }

            var employees = await _employeeService.GetDataOnlyAsync();
            
            var projectsDtos = _mapper.Map<List<ProjectReadDto>>(projects);
            var employeesDtos = _mapper.Map<List<EmployeeReadDto>>(employees);

            var model = new AddTaskViewModel() 
            {
                SelectedProjectId = id,
                Projects = projectsDtos,
                Employees = employeesDtos
            };

            return View(model);
        }

        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<IActionResult> Details(int id)
        {
            TaskReadDto? taskReadDto = await GetById(id);
            if (taskReadDto == null)
            {
                return NotFound();
            }

            var taskUpdateDto = _mapper.Map<TaskUpdateDto>(taskReadDto);

            List<Project> projects;
            int userId;
            var userRole = ControllersExtencions.GetUserRole(User);

            if (userRole <= EmployeeRole.Manager)
            {
                userId = ControllersExtencions.GetUserId(User);
                projects = await _employeeService.GetEmployeeAllProjectsByIdNoTrackingAsync(userId);
            }
            else
            {
                projects = await _projectService.GetDataOnlyAsync();
            }

            var employees = await _employeeService.GetDataOnlyAsync();

            var projectsDtos = _mapper.Map<List<ProjectReadDto>>(projects);
            var employeesDtos = _mapper.Map<List<EmployeeReadDto>>(employees);

            var model = new DetailsTaskViewModel()
            {
                Id = id,
                Task = taskUpdateDto,
                Projects = projectsDtos,
                Employees = employeesDtos
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult> Get(GetPagedModel model)
        {
            var builtParams = ControllersExtencions.BuildGetMethodModelParams(model);

            int? userId = null;
            var userRole = ControllersExtencions.GetUserRole(User);

            if (userRole <= EmployeeRole.Manager)
            {
                userId = ControllersExtencions.GetUserId(User);
            }

            var tasksPaged = await _service.GetPaginatedAsync(
                filterParams: builtParams.filterParams,
                sortParams: builtParams.sortParams,
                pageParams: builtParams.pageParams,
                employeeId: userId);

            var tasksDtos = _mapper.Map<List<TaskReadDto>>(tasksPaged.Datas);

            var result = new PagedResult<TaskReadDto>()
            {
                Datas = tasksDtos,
                FilteredRecords = tasksPaged.FilteredRecords,
                TotalRecords = tasksPaged.TotalRecords
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
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult> Add(TaskAddDto task)
        {
            var taskEntity = _mapper.Map<TaskPL>(task);
            await _service.AddAsync(taskEntity);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult> Update([FromRoute] int id, TaskUpdateDto task)
        {
            var taskEntity = _mapper.Map<TaskPL>(task);
            taskEntity.Id = id;
            await _service.UpdateAsync(taskEntity);

            return RedirectToAction("Index");
        }

        [HttpDelete]
        [Authorize(Policy = PolicyLevelName.BaseLevel)]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        private async Task<TaskReadDto> GetById(int id)
        {
            var task = await _service.GetByIdNoTrackingAsync(id);
            var taskDto = _mapper.Map<TaskReadDto>(task);
            return taskDto;
        }
    }
}
