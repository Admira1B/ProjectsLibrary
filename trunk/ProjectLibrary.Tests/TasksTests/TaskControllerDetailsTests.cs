using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.MVC.Models.Task;

namespace ProjectLibrary.Tests.TasksTests {
    public class TaskControllerDetailsTests : TasksControllerTests {
        [Fact]
        public async Task Details_WithValidData_RedirectsToIndex() {
            var model = new DetailsTaskViewModel { 
                Id = 1,
                Task = new TaskUpdateDto { Name = "Updated Task", ProjectId = 1 }
            };

            var task = new TaskPL();

            _mapper.Setup(x => x.Map<TaskPL>(model.Task)).Returns(task);
            _taskService.Setup(x => x.UpdateAsync(task)).Returns(Task.CompletedTask);

            var result = await _controller.Details(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);

            _mapper.Verify(x => x.Map<TaskPL>(model.Task), Times.Once);
            _taskService.Verify(x => x.UpdateAsync(task), Times.Once);
        }
    }
}
