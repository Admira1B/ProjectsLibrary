using AutoMapper;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.Domain.Models.Enums;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.DTOs.Project;
using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.MVC.Helpers;
using ProjectsLibrary.MVC.Models.Task;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;
using System.Security.Claims;

namespace ProjectsLibrary.MVC.ViewModelBuilders {
    public class TaskViewModelBuilder(ITaskService service, IProjectService projectService, IEmployeeService employeeService, IMapper mapper) : ITaskViewModelBuilder {
        private readonly ITaskService _service = service;
        private readonly IProjectService _projectService = projectService;
        private readonly IEmployeeService _employeeService = employeeService;
        private readonly IMapper _mapper = mapper;

        public Task<IndexTaskViewModel> BuildIndexViewModelAsync(ClaimsPrincipal user) {
            var model = new IndexTaskViewModel() {
                UserRole = UserHelper.GetUserRole(user)
            };

            return Task.FromResult(model);
        }

        public async Task<AddTaskViewModel> BuildAddViewModelAsync(ClaimsPrincipal user, int? id = null, TaskAddDto? taskDto = null) {
            List<Project> projects;
            int userId;
            var userRole = UserHelper.GetUserRole(user);

            if (userRole <= EmployeeRole.Manager) {
                userId = UserHelper.GetUserId(user);
                projects = await _employeeService.GetEmployeeAllProjectsByIdNoTrackingAsync(userId);
            } else {
                projects = await _projectService.GetDataOnlyAsync();
            }

            var employees = await _employeeService.GetDataOnlyAsync();

            var model = new AddTaskViewModel() {
                SelectedProjectId = id,
                Task = taskDto,
                Projects = _mapper.Map<List<ProjectReadDto>>(projects),
                Employees = _mapper.Map<List<EmployeeReadDto>>(employees)
            };

            return model;
        }

        public async Task<DetailsTaskViewModel?> BuildDetailsViewModelAsync(int id, ClaimsPrincipal user, TaskUpdateDto? taskDto = null) {
            var task = await _service.GetByIdNoTrackingAsync(id);

            if (task == null) {
                return null;
            }

            var taskUpdateDto = taskDto ?? _mapper.Map<TaskUpdateDto>(task);

            List<Project> projects;
            int userId;
            var userRole = UserHelper.GetUserRole(user);

            if (userRole <= EmployeeRole.Manager) {
                userId = UserHelper.GetUserId(user);
                projects = await _employeeService.GetEmployeeAllProjectsByIdNoTrackingAsync(userId);
            } else {
                projects = await _projectService.GetDataOnlyAsync();
            }

            var employees = await _employeeService.GetDataOnlyAsync();
            var taskCreator = await _employeeService.GetByIdNoTrackingAsync(taskUpdateDto.CreatorId);

            var model = new DetailsTaskViewModel() {
                Id = id,
                Task = taskUpdateDto,
                Projects = _mapper.Map<List<ProjectReadDto>>(projects),
                Employees = _mapper.Map<List<EmployeeReadDto>>(employees),
                TaskCreator = _mapper.Map<EmployeeShortDto>(taskCreator)
            };

            return model;
        }
    }
}
