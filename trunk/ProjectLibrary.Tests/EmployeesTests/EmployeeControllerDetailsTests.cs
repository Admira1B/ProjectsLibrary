using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.MVC.Models.Employee;
using ProjectsLibrary.DTOs.Employee;

namespace ProjectLibrary.Tests.EmployeesTests {
    public class EmployeeControllerDetailsTests : EmployeeControllerTests {
        [Fact]
        public async Task Update_WithValidData_RedirectsToIndex() {
            var model = new DetailsEmployeeViewModel {
                Id = 1,
                Employee = new EmployeeUpdateDto {
                    FirstName = "John",
                    LastName = "Updated",
                    Email = "john@test.com",
                    Password = "Welcome"
                }
            };

            var employee = new Employee {
                Id = 1,
                FirstName = "John",
                LastName = "Updated",
                Email = "john@test.com"
            };

            _mapper.Setup(x => x.Map<Employee>(model.Employee)).Returns(employee);
            _employeeService.Setup(x => x.UpdateAsync(employee)).Returns(Task.CompletedTask);

            var result = await _controller.Details(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);

            _mapper.Verify(x => x.Map<Employee>(model.Employee), Times.Once);
            _employeeService.Verify(x => x.UpdateAsync(employee), Times.Once);
        }
    }
}
