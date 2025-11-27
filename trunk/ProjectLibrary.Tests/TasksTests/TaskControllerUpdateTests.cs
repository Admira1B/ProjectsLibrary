using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.Domain.Models.Entities;

namespace ProjectLibrary.Tests.TasksTests {
    public class TaskControllerUpdateTests : TasksControllerTests {
        [Fact]
        public async Task Update_WithValidData_RedirectsToIndex() {
            var id = 1;
            var taskUpdateDto = new TaskUpdateDto { Name = "Updated Task", ProjectId = 1 };
            var taskEntity = new TaskPL { Id = id, Name = "Updated Task", ProjectId = 1 };

            _mapper.Setup(x => x.Map<TaskPL>(taskUpdateDto))
                   .Returns(taskEntity);
            _taskService.Setup(x => x.UpdateAsync(taskEntity))
                   .Returns(Task.CompletedTask);

            var result = await _controller.Update(id, taskUpdateDto);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal(id, taskEntity.Id);

            _mapper.Verify(x => x.Map<TaskPL>(taskUpdateDto), Times.Once);
            _taskService.Verify(x => x.UpdateAsync(taskEntity), Times.Once);
        }
    }
}
