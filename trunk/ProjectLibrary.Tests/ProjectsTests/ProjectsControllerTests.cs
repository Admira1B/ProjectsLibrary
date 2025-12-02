using Moq;
using AutoMapper;
using ProjectsLibrary.MVC.Controllers;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;
using ProjectsLibrary.Domain.Contracts.Services;

namespace ProjectLibrary.Tests.ProjectsTests {
    public class ProjectsControllerTests {
        protected readonly Mock<IMapper> _mapper;
        protected readonly Mock<IProjectService> _projectService;
        protected readonly Mock<IEmployeeService> _employeeService;
        protected readonly Mock<ICompanyService> _companyService;
        protected readonly Mock<IProjectViewModelBuilder> _viewModelBuilder;
        protected readonly ProjectsController _controller;
        protected readonly ProjectsLibrary.MVC.Controllers.Api.ProjectsController _apiController;

        public ProjectsControllerTests() {
            _mapper = new Mock<IMapper>();
            _projectService = new Mock<IProjectService>();
            _employeeService = new Mock<IEmployeeService>();
            _companyService = new Mock<ICompanyService>();
            _viewModelBuilder = new Mock<IProjectViewModelBuilder>();
            _controller = new ProjectsController(_viewModelBuilder.Object, _projectService.Object, _mapper.Object);
            _apiController = new ProjectsLibrary.MVC.Controllers.Api.ProjectsController(_projectService.Object, _mapper.Object);
        }
    }
}
