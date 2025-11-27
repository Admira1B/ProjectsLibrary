using Moq;
using Microsoft.AspNetCore.Mvc;

namespace ProjectLibrary.Tests.EmployeesTests {
    public class EmployeeControllerAssignTests : EmployeeControllerTests {
        [Fact]
        public async Task AssignTaskToEmployee_WithValidIds_ReturnsNoContent() {
            var employeeId = 1;
            var taskId = 100;

            _employeeService.Setup(s => s.AssignTaskToEmployee(employeeId, taskId))
                .Returns(Task.CompletedTask);

            var result = await _controller.AssignTaskToEmployee(employeeId, taskId);

            Assert.IsType<NoContentResult>(result);
            _employeeService.Verify(s => s.AssignTaskToEmployee(employeeId, taskId), Times.Once);
        }

        [Fact]
        public async Task UnassignTaskFromEmployee_WithValidIds_ReturnsNoContent() {
            var employeeId = 1;
            var taskId = 100;

            _employeeService.Setup(s => s.UnassignTaskToEmployee(employeeId, taskId))
                .Returns(Task.CompletedTask);

            var result = await _controller.UnassignTaskFromEmployee(employeeId, taskId);

            Assert.IsType<NoContentResult>(result);
            _employeeService.Verify(s => s.UnassignTaskToEmployee(employeeId, taskId), Times.Once);
        }
    }
}
