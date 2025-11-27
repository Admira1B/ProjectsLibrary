using Moq;
using Microsoft.AspNetCore.Mvc;

namespace ProjectLibrary.Tests.TasksTests {
    public class TaskControllerDeleteTests : TasksControllerTests {
        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent() {
            var id = 1;
            _taskService.Setup(x => x.DeleteAsync(id))
                   .Returns(Task.CompletedTask);

            var result = await _controller.Delete(id);

            Assert.IsType<NoContentResult>(result);
            _taskService.Verify(x => x.DeleteAsync(id), Times.Once);
        }
    }
}
