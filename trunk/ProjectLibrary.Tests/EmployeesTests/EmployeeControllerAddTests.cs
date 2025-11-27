using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectLibrary.Tests.EmployeesTests {
    public class EmployeeControllerAddTests : EmployeeControllerTests {
        [Fact]
        public async Task Add_WithValidEmployee_RedirectsToIndex() {
            var employeeAddDto = new EmployeeAddDto { FirstName = "John", LastName = "Doe", Email = "john@test.com", Password = "Welcome" };
            var employeeEntity = new Employee { FirstName = "John", LastName = "Doe", Email = "john@test.com" };

            _mapper.Setup(x => x.Map<Employee>(employeeAddDto))
                   .Returns(employeeEntity);
            _employeeService.Setup(x => x.AddAsync(employeeEntity))
                   .Returns(Task.CompletedTask);

            var result = await _controller.Add(employeeAddDto);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);

            _mapper.Verify(x => x.Map<Employee>(employeeAddDto), Times.Once);
            _employeeService.Verify(x => x.AddAsync(employeeEntity), Times.Once);
        }
    }
}
