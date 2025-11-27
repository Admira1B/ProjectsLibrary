using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.MVC.Models.Home;
using ProjectsLibrary.Domain.Exceptions;

namespace ProjectLibrary.Tests.HomeTests {
    public class HomeControllerLoginTests : HomeControllerTests 
    {
        [Fact]
        public async Task Login_ValidEmployee_RedirectsToTasks() {
            var employeeDto = new EmployeeLoginDto()
            {
                Email = "testemail@test.te",
                Password = "qwertyQAZ123",
            };

            string token = "valid.jwt.token";

            var httpContext = new DefaultHttpContext();

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            _employeeService.Setup(s => s.LoginAsync(employeeDto.Email, employeeDto.Password))
                           .ReturnsAsync(token);

            var result = await _controller.Login(employeeDto);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Tasks", redirectResult.ControllerName);

            var setCookieHeader = httpContext.Response.Headers.SetCookie.ToString();

            Assert.Contains("auth-t", setCookieHeader);
            Assert.Contains(token, setCookieHeader);

            _employeeService.Verify(s => s.LoginAsync(employeeDto.Email, employeeDto.Password), Times.Once);
        }

        [Fact]
        public async Task Login_NotRegistredEmail_ReturnsViewWithError() 
        {
            var employeeDto = new EmployeeLoginDto()
            {
                Email = "testemail@test.te",
                Password = "qwertyQAZ123",
            };

            var exceptionMessage = "Invalid email";

            _employeeService.Setup(s => s.LoginAsync(employeeDto.Email, employeeDto.Password))
                            .ThrowsAsync(new EmailNotRegisteredException(exceptionMessage));

            var result = await _controller.Login(employeeDto);
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<LoginViewModel>(viewResult.Model);

            Assert.Equal(employeeDto, model.Employee);

            Assert.True(_controller.ModelState.ContainsKey(string.Empty));

            var errors = _controller.ModelState[string.Empty]?.Errors;
            Assert.NotNull(errors);
            Assert.Contains(errors, e => e.ErrorMessage == exceptionMessage);

            _employeeService.Verify(s => s.LoginAsync(employeeDto.Email, employeeDto.Password), Times.Once);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsViewWithError() 
        {
            var employeeDto = new EmployeeLoginDto()
            {
                Email = "testemail@test.te",
                Password = "qwertyQAZ123",
            };

            var exceptionMessage = "Invalid password";

            _employeeService.Setup(s => s.LoginAsync(employeeDto.Email, employeeDto.Password))
                            .ThrowsAsync(new IncorrectEmployeePasswordException(exceptionMessage));

            var result = await _controller.Login(employeeDto);
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<LoginViewModel>(viewResult.Model);

            Assert.Equal(employeeDto, model.Employee);

            Assert.True(_controller.ModelState.ContainsKey(string.Empty));

            var errors = _controller.ModelState[string.Empty]?.Errors;
            Assert.NotNull(errors);
            Assert.Contains(errors, e => e.ErrorMessage == exceptionMessage);

            _employeeService.Verify(s => s.LoginAsync(employeeDto.Email, employeeDto.Password), Times.Once);
        }

        [Fact]
        public async Task Login_CreatedButNotRegistredEmployee_ReturnsViewWithError() 
        {
            var employeeDto = new EmployeeLoginDto()
            {
                Email = "testemail@test.te",
                Password = "qwertyQAZ123",
            };

            var exceptionMessage = "Employee not registered";

            _employeeService.Setup(s => s.LoginAsync(employeeDto.Email, employeeDto.Password))
                            .ThrowsAsync(new CreatedEmployeeNotRegisteredException(exceptionMessage));

            var result = await _controller.Login(employeeDto);
            var viewResult = Assert.IsType<ViewResult>(result);

            var model = Assert.IsType<LoginViewModel>(viewResult.Model);

            Assert.Equal(employeeDto, model.Employee);

            Assert.True(_controller.ModelState.ContainsKey(string.Empty));

            var errors = _controller.ModelState[string.Empty]?.Errors;
            Assert.NotNull(errors);
            Assert.Contains(errors, e => e.ErrorMessage == exceptionMessage);

            _employeeService.Verify(s => s.LoginAsync(employeeDto.Email, employeeDto.Password), Times.Once);
        }
    }
}
