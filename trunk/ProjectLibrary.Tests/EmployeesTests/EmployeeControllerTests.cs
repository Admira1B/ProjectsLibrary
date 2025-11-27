using Moq;
using AutoMapper;
using ProjectsLibrary.MVC.Controllers;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;
using ProjectsLibrary.Domain.Contracts.Services;

namespace ProjectLibrary.Tests.EmployeesTests
{
    public class EmployeeControllerTests
    {
        protected readonly Mock<IMapper> _mapper;
        protected readonly Mock<IEmployeeService> _employeeService;
        protected readonly Mock<IProjectService> _projectService;
        protected readonly Mock<IEmployeeViewModelBuilder> _viewModelBuilder;
        protected readonly EmployeesController _controller;

        public EmployeeControllerTests()
        {
            _mapper = new Mock<IMapper>();
            _employeeService = new Mock<IEmployeeService>();
            _projectService = new Mock<IProjectService>();
            _viewModelBuilder = new Mock<IEmployeeViewModelBuilder>();

        _controller = new EmployeesController(_viewModelBuilder.Object, _employeeService.Object, _projectService.Object, _mapper.Object);
        }
    }
}
