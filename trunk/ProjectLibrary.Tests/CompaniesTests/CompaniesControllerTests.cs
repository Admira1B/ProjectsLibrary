using Moq;
using AutoMapper;
using ProjectsLibrary.MVC.Controllers;
using ProjectsLibrary.Domain.Contracts.Services;
using ProjectsLibrary.MVC.ViewModelBuilders.Interfaces;

namespace ProjectLibrary.Tests.CompaniesTests {
    public abstract class CompaniesControllerTests {
        protected readonly Mock<IMapper> _mapper;
        protected readonly Mock<ICompanyService> _companyService;
        protected readonly Mock<ICompanyViewModelBuilder> _viewModelBuilder;
        protected readonly CompaniesController _controller;

        public CompaniesControllerTests() {
            _mapper = new Mock<IMapper>();
            _companyService = new Mock<ICompanyService>();
            _viewModelBuilder = new Mock<ICompanyViewModelBuilder>();
            _controller = new CompaniesController(_viewModelBuilder.Object, _companyService.Object, _mapper.Object);
        }
    }
}
