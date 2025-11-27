using Moq;
using AutoMapper;
using ProjectsLibrary.MVC.Controllers;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;

namespace ProjectLibrary.Tests.TasksTests {
    public abstract class TasksControllerTests {
        protected readonly Mock<IMapper> _mapper;
        protected readonly Mock<ITaskService> _taskService;
        protected readonly Mock<IProjectService> _projectService;
        protected readonly Mock<IEmployeeService> _employeeService;
        protected readonly Mock<ITaskViewModelBuilder> _viewModelBuilder;
        protected readonly TasksController _controller;

        public TasksControllerTests() {
            _mapper = new Mock<IMapper>();
            _taskService = new Mock<ITaskService>();
            _projectService = new Mock<IProjectService>();
            _employeeService = new Mock<IEmployeeService>();
            _viewModelBuilder = new Mock<ITaskViewModelBuilder>();
            _controller = new TasksController(_viewModelBuilder.Object, _taskService.Object, _mapper.Object);
        }
    }
}
