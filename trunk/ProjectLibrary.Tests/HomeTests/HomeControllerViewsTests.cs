using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.MVC.Models.Home;

namespace ProjectLibrary.Tests.HomeTests 
{
    public class HomeControllerViewsTests : HomeControllerTests 
    {
        [Fact]
        public void Index_WhenCalled_ReturnsView() 
        {
            var result = _controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Login_WhenCalled_ReturnsView() 
        {
            var model = new LoginViewModel();

            _viewModelBuilder.Setup(b => b.BuildLoginViewModelAsync(null)).ReturnsAsync(model);

            var result = await _controller.Login();

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Register_WithValidEmail_ReturnsView() 
        {
            var email = "testemail@test.te";
            var model = new RegistrationViewModel();

            _viewModelBuilder.Setup(b => b.BuildRegistrationViewModelAsync(email, null)).ReturnsAsync(model);

            var result = await _controller.Registration(email);

            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Register_WithEmptyEmail_ReturnsView() 
        {
            var email = "";
            var model = new RegistrationViewModel();

            _viewModelBuilder.Setup(b => b.BuildRegistrationViewModelAsync(email, null)).ReturnsAsync(model);

            var result = await _controller.Registration(email);

            Assert.IsType<ViewResult>(result);
        }
    }
}
