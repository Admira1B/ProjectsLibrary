using Moq;
using Microsoft.AspNetCore.Mvc;

namespace ProjectLibrary.Tests.EmployeesTests {
    public class EmployeeControllerDeleteTests : EmployeeControllerTests {
        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent() {
            var id = 1;
            _employeeService.Setup(x => x.DeleteAsync(id))
                   .Returns(Task.CompletedTask);

            var result = await _controller.Delete(id);

            Assert.IsType<NoContentResult>(result);
            _employeeService.Verify(x => x.DeleteAsync(id), Times.Once);
        }
    }
}
