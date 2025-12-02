using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.MVC.Models.Home;
using ProjectsLibrary.Domain.Exceptions;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectLibrary.Tests.HomeTests {
    public class HomeControllerRegistrationTests : HomeControllerTests {
        [Fact]
        public async Task Registration_ValidEmployee_RedirectsToLogin() {
            var model = new RegistrationViewModel() {
                Employee = new EmployeeAddDto() {
                    Email = "testemail@test.te",
                    FirstName = "Jhon",
                    LastName = "Doe",
                    Password = "qwertyQAZ123"
                }
            };

            var employee = new Employee();

            _mapper.Setup(m => m.Map<Employee>(model.Employee)).Returns(employee);
            _employeeService.Setup(s => s.RegisterAsync(employee, model.Employee.Password)).Returns(Task.CompletedTask);

            var result = await _controller.Registration(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Login", redirectResult.ActionName);

            _mapper.Verify(m => m.Map<Employee>(model.Employee), Times.Once);
            _employeeService.Verify(s => s.RegisterAsync(employee, model.Employee.Password), Times.Once);
        }

        [Fact]
        public async Task Registration_ExistingEmployee_ReturnsViewWithError() {
            var model = new RegistrationViewModel() {
                Employee = new EmployeeAddDto() {
                    Email = "testemail@test.te",
                    FirstName = "Jhon",
                    LastName = "Doe",
                    Password = "qwertyQAZ123"
                }
            };

            var employee = new Employee();

            var exceptionMessage = "Employee already exists";

            _mapper.Setup(m => m.Map<Employee>(model.Employee)).Returns(employee);
            _employeeService.Setup(s => s.RegisterAsync(employee, model.Employee.Password))
                            .ThrowsAsync(new EmployeeAlreadyExistsException(exceptionMessage));

            var result = await _controller.Registration(model);

            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.True(_controller.ModelState.ContainsKey(string.Empty));

            var errors = _controller.ModelState[string.Empty]?.Errors;
            Assert.NotNull(errors);
            Assert.Contains(errors, e => e.ErrorMessage == exceptionMessage);

            _mapper.Verify(m => m.Map<Employee>(model.Employee), Times.Once);
            _employeeService.Verify(s => s.RegisterAsync(employee, model.Employee.Password), Times.Once);
        }
    }
}
