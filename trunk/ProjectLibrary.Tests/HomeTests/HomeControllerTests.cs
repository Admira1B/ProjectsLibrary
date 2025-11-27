using Moq;
using AutoMapper;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.MVC.Controllers;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;

namespace ProjectLibrary.Tests.HomeTests
{
    public abstract class HomeControllerTests 
    {
        protected readonly Mock<IMapper> _mapper;
        protected readonly Mock<IEmployeeService> _employeeService;
        protected readonly Mock<IHomeViewModelBuilder> _viewModelBuilder;
        protected readonly HomeController _controller;

        public HomeControllerTests() 
        {
            _mapper = new Mock<IMapper>();
            _employeeService = new Mock<IEmployeeService>();
            _viewModelBuilder = new Mock<IHomeViewModelBuilder>();
            _controller = new HomeController(_viewModelBuilder.Object, _mapper.Object, _employeeService.Object);
        }
    }
}
