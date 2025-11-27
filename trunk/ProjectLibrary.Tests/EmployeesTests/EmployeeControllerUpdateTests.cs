using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Employee;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectLibrary.Tests.EmployeesTests {
    public class EmployeeControllerUpdateTests : EmployeeControllerTests {
        [Fact]
        public async Task Update_WithValidData_RedirectsToIndex() {
            var id = 1;
            var employeeUpdateDto = new EmployeeUpdateDto { FirstName = "John", LastName = "Updated", Email = "john@test.com", Password = "Welcome" };
            var employeeEntity = new Employee { Id = id, FirstName = "John", LastName = "Updated", Email = "john@test.com" };

            _mapper.Setup(x => x.Map<Employee>(employeeUpdateDto))
                   .Returns(employeeEntity);
            _employeeService.Setup(x => x.UpdateAsync(employeeEntity))
                   .Returns(Task.CompletedTask);

            var result = await _controller.Update(id, employeeUpdateDto);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal(id, employeeEntity.Id);

            _mapper.Verify(x => x.Map<Employee>(employeeUpdateDto), Times.Once);
            _employeeService.Verify(x => x.UpdateAsync(employeeEntity), Times.Once);
        }
    }
}
