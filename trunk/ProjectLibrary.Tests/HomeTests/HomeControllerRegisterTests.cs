using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.MVC.Models.Home;
using ProjectsLibrary.Domain.Exceptions;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectLibrary.Tests.HomeTests 
{
    public class HomeControllerRegisterTests : HomeControllerTests 
    {
        [Fact]
        public async Task Register_ValidEmployee_RedirectsToLogin() {
            var employeeDto = new EmployeeAddDto()
            {
                Email = "testemail@test.te",
                FirstName = "Jhon",
                LastName = "Doe",
                Password = "qwertyQAZ123"
            };

            var employeeEntity = new Employee();

            _mapper.Setup(m => m.Map<Employee>(employeeDto)).Returns(employeeEntity);
            _employeeService.Setup(s => s.RegisterAsync(employeeEntity, employeeDto.Password))
                           .Returns(Task.CompletedTask);

            var result = await _controller.Register(employeeDto); 

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Login", redirectResult.ActionName);

            _mapper.Verify(m => m.Map<Employee>(employeeDto), Times.Once);
            _employeeService.Verify(s => s.RegisterAsync(employeeEntity, employeeDto.Password), Times.Once);
        }

        [Fact]
        public async Task Register_ExistingEmployee_ReturnsViewWithError() 
        {
            var employeeDto = new EmployeeAddDto
            {
                Email = "testemail@test.te",
                FirstName = "Jhon",
                LastName = "Doe",
                Password = "qwertyQAZ123"
            };

            var employeeEntity = new Employee();
            var exceptionMessage = "Employee already exists";

            _mapper.Setup(m => m.Map<Employee>(employeeDto))
                   .Returns(employeeEntity);
            _employeeService.Setup(s => s.RegisterAsync(employeeEntity, employeeDto.Password))
                            .ThrowsAsync(new EmployeeAlreadyExistsException(exceptionMessage));

            var result = await _controller.Register(employeeDto);

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Registration", viewResult.ViewName);

            var model = Assert.IsType<RegistrationViewModel>(viewResult.Model);
            Assert.Equal(employeeDto, model.Employee);

            Assert.True(_controller.ModelState.ContainsKey(string.Empty));

            var errors = _controller.ModelState[string.Empty]?.Errors;
            Assert.NotNull(errors);
            Assert.Contains(errors, e => e.ErrorMessage == exceptionMessage);

            _mapper.Verify(m => m.Map<Employee>(employeeDto), Times.Once);
            _employeeService.Verify(s => s.RegisterAsync(employeeEntity, employeeDto.Password), Times.Once);
        }
    }
}
