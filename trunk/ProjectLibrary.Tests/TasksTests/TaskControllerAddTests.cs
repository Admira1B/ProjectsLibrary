using Moq;
using Microsoft.AspNetCore.Mvc;
using ProjectsLibrary.DTOs.Task;
using ProjectsLibrary.Domain.Models.Entities;
using ProjectsLibrary.MVC.Models.Task;

namespace ProjectLibrary.Tests.TasksTests {
    public class TaskControllerAddTests : TasksControllerTests {
        [Fact]
        public async Task Add_WithValidDto_ReturnsRedirectToIndex() {
            var model = new AddTaskViewModel { 
                Task = new TaskAddDto {
                    Name = "New Task",
                }
            };

            var task = new TaskPL();

            _mapper.Setup(m => m.Map<TaskPL>(model.Task)).Returns(task);

            _taskService.Setup(s => s.AddAsync(task)).Returns(Task.CompletedTask);

            var result = await _controller.Add(model);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(_controller.Index), redirectResult.ActionName);

            _mapper.Verify(m => m.Map<TaskPL>(model.Task), Times.Once);
            _taskService.Verify(s => s.AddAsync(task), Times.Once);
        }
    }
}
